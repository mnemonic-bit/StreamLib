using System;
using System.IO;

namespace StreamLib
{
    /// <summary>
    /// Wraps the <code>Stream</code> API around a <code>ReadOnlyMemory</code> reference.
    /// As an <code>ArraySegment</code> can be cast implicitly to <code>ReadOnlyMemory</code>,
    /// We only provide this class as an API wrapper.
    /// </summary>
    public sealed class ReadOnlyMemoryStream : Stream
    {

        public ReadOnlyMemoryStream(ReadOnlyMemory<byte> memory)
        {
            _memory = memory;
        }

        public override bool CanRead => true;

        public override bool CanSeek => true;

        public override bool CanWrite => false;

        public override long Length => _memory.Length;

        public override long Position
        {
            get { return _position; }
            set
            {
                Seek(value, SeekOrigin.Begin);
            }
        }

        public override void Flush()
        {
            // Do nothing here.
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int cnt = count;

            if (offset + cnt > buffer.Length)
            {
                cnt = buffer.Length - offset - 1;
            }

            if (_position + cnt > _memory.Length)
            {
                cnt = _memory.Length - offset - 1;
            }

            _memory.Slice(_position, cnt).CopyTo(buffer.AsMemory(offset));
            _position += cnt;

            return cnt;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            var newPosition = origin switch
            {
                SeekOrigin.Begin => offset,
                SeekOrigin.Current => _position + offset,
                SeekOrigin.End => _memory.Length - (int)offset,
                _ => throw new ArgumentOutOfRangeException(nameof(origin)),
            };

            EnsurePositionIsValid(newPosition);

            _position = (int)newPosition;

            return _position;
        }

        public override void SetLength(long value)
        {
            throw new InvalidOperationException("The underlying ReadOnlyMemory is read-only. This inludes setting its length.");
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new InvalidOperationException("The underlying ReadOnlyMemory is read-only.");
        }


        private int _position = 0;
        private readonly ReadOnlyMemory<byte> _memory;


        private void EnsurePositionIsValid(long position)
        {
            if (position < 0)
            {
                throw new ArgumentException($"The given offset would move the current position out of bounds of the underlying ReadonlyMemory. The new calculated position would be {position}, which moves past the lower limit of the read-only memory.");
            }

            if (position > Int32.MaxValue)
            {
                throw new ArgumentException($"The given offset would move the current position out of bounds of the underlying ReadonlyMemory. The new calculated position would be {position}, while the read-only memory has a length of {_memory.Length}.");
            }
        }

    }
}
