using StreamLib.Implementation;
using System;
using System.IO;

namespace StreamLib
{
    /// <summary>
    /// A MeteringStream can be used to measure the amount of data
    /// that is read from or writtenn to a base stream. It wraps around
    /// an instance of Stream and provides the same interface, hence it
    /// can be used as a replacement of the original base stream with
    /// the addition of speed-measurement features.
    /// 
    /// To measure the progress or speed of read or write operations on
    /// the base stream, calls to the base-stream's read and write methods
    /// are chunked, to give the caller of this stream steady feedback.
    /// </summary>
    public sealed class MeteringStream : Stream
    {

        /// <summary>
        /// Creates a new wrapper stream around a base stream with a given
        /// metering-interval length.
        /// </summary>
        /// <param name="baseStream">The base stream.</param>
        /// <param name="intervalLength">The length of a metering interval in milliseconds.</param>
        public MeteringStream(Stream baseStream, int intervalLength = 1000)
        {
            _baseStream = baseStream;

            _meteringReadOperation = new MeteringOperation(
                intervalLength,
                _baseStream.Read,
                (a, b) => a < b);

            _meteringWriteOperation = new MeteringOperation(
                intervalLength,
                (b, o, c) =>
                {
                    _baseStream.Write(b, o, c);
                    return c;
                },
                (a, b) => true);
        }

        public override bool CanRead => _baseStream.CanRead;

        public override bool CanSeek => _baseStream.CanSeek;

        public override bool CanWrite => _baseStream.CanWrite;

        public override long Length => _baseStream.Length;

        public override long Position { get => _baseStream.Position; set => _baseStream.Position = value; }

        /// <summary>
        /// Adds the same event listener as read and write listener.
        /// </summary>
        /// <param name="eventListener">The listener to be added.</param>
        public void AddEventListener(Action<MeteringEventArgs> eventListener)
        {
            AddReadEventListener(eventListener);
            AddWriteEventListener(eventListener);
        }

        public void AddReadEventListener(Action<MeteringEventArgs> eventListener)
        {
            _meteringReadOperation.OnProgress += eventListener;
        }

        public void AddWriteEventListener(Action<MeteringEventArgs> eventListener)
        {
            _meteringWriteOperation.OnProgress += eventListener;
        }

        public override void Flush()
        {
            _baseStream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return _meteringReadOperation.MeterOperation(buffer, offset, count);
        }

        public void RemoveEventListener(Action<MeteringEventArgs> eventListener)
        {
            RemovReadEventListener(eventListener);
            RemoveWriteEventListener(eventListener);
        }

        public void RemovReadEventListener(Action<MeteringEventArgs> eventListener)
        {
            _meteringReadOperation.OnProgress -= eventListener;
        }

        public void RemoveWriteEventListener(Action<MeteringEventArgs> eventListener)
        {
            _meteringWriteOperation.OnProgress -= eventListener;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return _baseStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            _baseStream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _meteringWriteOperation.MeterOperation(buffer, offset, count);
        }


        private readonly Stream _baseStream;

        private readonly MeteringOperation _meteringReadOperation;
        private readonly MeteringOperation _meteringWriteOperation;


    }
}
