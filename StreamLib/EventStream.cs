using System;
using System.IO;

namespace StreamLib
{
    /// <summary>
    /// The <code>EventStream</code> adds introspection capabilities to a base-stream
    /// by wrapping every call to the base-stream and adding events to each call that
    /// a client can register to.
    /// </summary>
    public class EventStream : Stream
    {

        public EventStream(Stream baseStream)
        {
            _baseStream = baseStream;
        }

        public event Action<bool>? OnCanRead;
        public event Action<bool>? OnCanSeek;
        public event Action<long>? OnGetPosition;
        public event Action<long>? OnLength;
        public event Action<long, long>? OnSetPosition;
        public event Action? OnFlush;
        public event Action<byte[], int, int, int>? OnRead;
        public event Action<long, SeekOrigin, long>? OnSeek;
        public event Action<long, long>? OnSetLength;
        public event Action<byte[], int, int>? OnWrite;

        public override bool CanRead
        {
            get
            {
                var canRead = _baseStream.CanRead;

                if (OnCanRead != null) OnCanRead(canRead);

                return canRead;
            }
        }

        public override bool CanSeek
        {
            get
            {
                var canSeek = _baseStream.CanSeek;

                if (OnCanSeek != null) OnCanSeek(canSeek);

                return canSeek;
            }
        }

        public override bool CanWrite
        {
            get
            {
                var canWrite = _baseStream.CanWrite;

                if (OnCanSeek != null) OnCanSeek(canWrite);

                return canWrite;
            }
        }

        public override long Length
        {
            get
            {
                var length = _baseStream.Length;

                if (OnLength != null) OnLength(length);

                return length;
            }
        }

        public override long Position
        {
            get
            {
                var position = _baseStream.Position;

                if (OnGetPosition != null) OnGetPosition(position);

                return position;
            }
            set
            {
                var oldPosition = _baseStream.Position;

                _baseStream.Position = value;

                if (OnSetPosition != null) OnSetPosition(oldPosition, value);
            }
        }

        public override void Flush()
        {
            _baseStream.Flush();

            if (OnFlush != null) OnFlush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int bytesRead = _baseStream.Read(buffer, offset, count);

            if (OnRead != null) OnRead(buffer, offset, count, bytesRead);

            return bytesRead;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            var position = _baseStream.Seek(offset, origin);

            if (OnSeek != null) OnSeek(offset, origin, position);

            return position;
        }

        public override void SetLength(long value)
        {
            var oldLength = _baseStream.Length;

            _baseStream.SetLength(value);

            if (OnSetLength != null) OnSetLength(oldLength, value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _baseStream?.Write(buffer, offset, count);

            if (OnWrite != null) OnWrite(buffer, offset, count);
        }


        private readonly Stream _baseStream;

    }
}
