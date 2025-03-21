using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;

namespace StreamLib
{
    /// <summary>
    /// The <code>EphemeralStream</code> is used to provide an in-memory buffer with a
    /// <code>Stream</code> interface.
    /// </summary>
    /// <remarks>
    /// This stream implementation utilises the shared <code>ArrayPool</code> instance
    /// to its memory allocations. Allocation is done in chunks, rather than in a
    /// contigious block. This has some advantages like
    ///  1. It prevents allocation of memory on the large object heap for short amounts of time
    ///     if the amount exceeds a cretain threshold (a bit above 80k bytes)
    ///  2. No resizing of the underlying buffers is needed, as we always can add another
    ///     chunk of memory.
    ///  3. Memory allocation does not take place unless you actually access that
    ///     part of the stream.
    /// </remarks>
    public sealed class EphemeralStream : Stream
    {

        /// <summary>
        /// Initializes a new stream with a default number of chunks
        /// and a default chunk size.
        /// </summary>
        public EphemeralStream()
            : this(DefaultNumberOfChunks, DefaultChunkSize)
        {
        }

        /// <summary>
        /// Initializes a new stream with the given number of chunks
        /// and the given chunk size.
        /// </summary>
        /// <param name="numberOfChunks">The initial number of chunks this stream maintains.</param>
        /// <param name="chunkSize">The size of the chunks used internally to store the stream content</param>
        /// <param name="fixedSize">If set to true, this stream cannot expand or shrink its capacity</param>
        public EphemeralStream(int numberOfChunks, int chunkSize, bool fixedSize = false)
        {
            _chunkSize = chunkSize == 0 ? 1 : chunkSize;
            _fixedSize = fixedSize;
            _bufferChunks = ArrayPool<byte[]>.Shared.Rent(numberOfChunks);
            _position = 0;
        }

        public override bool CanRead => true;

        public override bool CanSeek => true;

        public override bool CanWrite => true;

        public override long Length => _capacity;

        public override long Position
        {
            get
            {
                return _position;
            }
            set
            {
                Seek(value, SeekOrigin.Begin);
            }
        }

        public new void Dispose()
        {
            // Return the borrowed buffers back to the BufferManager
            foreach (var buffer in _bufferChunks)
            {
                if (buffer != null)
                {
                    ArrayPool<byte>.Shared.Return(buffer);
                }
            }

            ArrayPool<byte[]>.Shared.Return(_bufferChunks);

            base.Dispose();
        }

        public override void Flush()
        {
            // Nothing to do here.
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (offset + count > buffer.Length)
            {
                throw new ArgumentException($"The given buffer parameter has a capacity of {buffer.Length} bytes, but starting from the offset {offset} will make accessing {count} bytes out of bounds in the given buffer.");
            }

            if (_position + count > _capacity)
            {
                count = (int)(_capacity - _position);
            }

            int readCount = 0;

            foreach (var chunk in GetChunks(_position, count))
            {
                if (chunk.Array == null)
                {
                    throw new Exception("Internal error: array-segment does not reference to an array.");
                }

                Buffer.BlockCopy(chunk.Array, chunk.Offset, buffer, offset, chunk.Count);
                readCount += chunk.Count;
                offset += chunk.Count;
                _position += chunk.Count;
            }

            return readCount;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            long newPosition = origin switch
            {
                SeekOrigin.Begin => offset,
                SeekOrigin.Current => _position + offset,
                SeekOrigin.End => _capacity - offset,
                _ => throw new Exception("Seek origin not implemented yet. Please contact the maintainer.")
            };

            EnsurePositionIsValid(newPosition);

            _position = newPosition;

            return newPosition;
        }

        public override void SetLength(long value)
        {
            if (value / _chunkSize > _bufferChunks.Length)
            {
                ResizeBufferChunks(value / _chunkSize);
            }

            _capacity = value;
        }

        /// <summary>
        /// Returns an array containing all contents of this stream so far.
        /// </summary>
        /// <returns></returns>
        public byte[] ToArray()
        {
            byte[] buffer = new byte[_capacity];

            int offset = 0;
            foreach (var chunk in GetChunks(0, _capacity))
            {
                Buffer.BlockCopy(chunk.Array!, chunk.Offset, buffer, offset, chunk.Count);
                offset += chunk.Count;
            }

            return buffer;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (offset + count > buffer.Length)
            {
                throw new ArgumentException($"The given buffer parameter contains {buffer.Length} bytes, but starting from the offset {offset} will make accessing {count} bytes out of bounds in the given buffer.");
            }

            if (_position + count > _capacity)
            {
                SetLength(_position + count);
            }

            foreach (var chunk in GetChunks(_position, count))
            {
                if (chunk.Array == null)
                {
                    throw new Exception("Internal error: array-segment does not reference to an array.");
                }

                Buffer.BlockCopy(buffer, offset, chunk.Array, chunk.Offset, chunk.Count);
                offset += chunk.Count;
                _position += chunk.Count;
            }
        }


        private const int DefaultNumberOfChunks = 128;
        private const int DefaultChunkSize = 4096;

        private readonly int _chunkSize;
        private readonly bool _fixedSize;
        private byte[][] _bufferChunks;
        private long _position;
        private long _capacity;


        private void EnsureBufferChunks(int chunkNumber)
        {
            if (chunkNumber < _bufferChunks.Length)
            {
                return;
            }

            ResizeBufferChunks(chunkNumber);
        }

        private byte[] EnsureChunk(int chunkNumber)
        {
            if (_bufferChunks[chunkNumber] == null)
            {
                _bufferChunks[chunkNumber] = ArrayPool<byte>.Shared.Rent(_chunkSize);
            }

            return _bufferChunks[chunkNumber];
        }

        private void EnsurePositionIsValid(long position)
        {
            // A negative number would indicate an overflow in integer
            // arithmetic, wich would mean that we have moved past the
            // upper bounds of our stream capabilities.
            if (position < 0)
            {
                throw new ArgumentException($"The given offset would move the current position out of bounds of the underlying memory buffers. The new calculated position would be {position}, which moves past the lower limit of the memory.");
            }
        }

        private (int, int) GetChunkPosition(long position)
        {
            //TODO: make this based on the integer type long instead?
            return ((int)(position / _chunkSize), (int)(position % _chunkSize));
        }

        private IEnumerable<ArraySegment<byte>> GetChunks(long offset, long count)
        {
            if (count == 0)
            {
                yield break;
            }

            (int startChunk, int startOffset) = GetChunkPosition(offset);
            (int endChunk, int endOffset) = GetChunkPosition(offset + count - 1);

            if (startChunk == endChunk)
            {
                byte[] chunk = EnsureChunk(startChunk);
                yield return new ArraySegment<byte>(chunk, startOffset, endOffset - startOffset + 1);
                yield break;
            }

            byte[] firstChunk = EnsureChunk(startChunk);
            yield return new ArraySegment<byte>(firstChunk, startOffset, _chunkSize - startOffset);

            for (int chunkNumber = startChunk + 1; chunkNumber < endChunk; chunkNumber++)
            {
                byte[] chunk = EnsureChunk(chunkNumber);
                yield return new ArraySegment<byte>(chunk, 0, _chunkSize);
            }

            byte[] lastChunk = EnsureChunk(endChunk);
            yield return new ArraySegment<byte>(lastChunk, 0, endOffset + 1);
        }

        private void ResizeBufferChunks(long? minimumNumberOfChunks = null)
        {
            if (_fixedSize)
            {
                throw new ArgumentOutOfRangeException("This stream cannot be resized.");
            }

            if (minimumNumberOfChunks.HasValue && minimumNumberOfChunks.Value > Int32.MaxValue)
            {
                throw new ArgumentOutOfRangeException($"The requested number of chunks to allocate exceeds this stream's capabilities. The allocation request was for {minimumNumberOfChunks.Value} chunks.");
            }

            int newNumberOfChunks = minimumNumberOfChunks.HasValue ? (int)minimumNumberOfChunks.Value : _bufferChunks.Length * 2;

            byte[][] oldBufferChunks = _bufferChunks;
            _bufferChunks = ArrayPool<byte[]>.Shared.Rent(newNumberOfChunks);
            Array.Copy(oldBufferChunks, _bufferChunks, oldBufferChunks.Length);
            ArrayPool<byte[]>.Shared.Return(_bufferChunks);
        }

    }
}
