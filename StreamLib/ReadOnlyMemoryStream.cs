using System;
using System.IO;

namespace StreamLib
{
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
                if (value > Int32.MaxValue)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "The given position exceeds the capabilities of an underlying ReadOnlyMemory.");
                }

                _position = (int)value;
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
            if (offset > Int32.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(offset), "The given position exceeds the capabilities of an underlying ReadOnlyMemory.");
            }

            switch (origin)
            {
                case SeekOrigin.Begin:
                    {
                        _position = (int)offset;
                        break;
                    }
                case SeekOrigin.Current:
                    {
                        _position += (int)offset;
                        break;
                    }
                case SeekOrigin.End:
                    {
                        _position = _memory.Length - (int)offset;
                        break;
                    }
            }

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
    }
}
