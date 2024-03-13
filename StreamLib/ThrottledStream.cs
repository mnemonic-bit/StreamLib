using StreamLib.Implementation;
using System.IO;

namespace StreamLib
{
    /// <summary>
    /// This class is a wrapper for a Stream instance which guarantees that
    /// the Read() or Write() methods of the base stream are not called with
    /// more load than a given limit per time interval. This results in limiting
    /// the number of bytes requested from the base stream to a maximum for any
    /// given time frame.
    /// 
    /// Should the client code which uses a ThrottledStream
    /// cause a pause for more than one time interval, throttling resumes as
    /// it would when the initial call is made, i.e. that the client can still
    /// slow down the overall speed of this throttled stream. In the case that
    /// the base stream provides only part of the requested number of bytes,
    /// this method returns early.
    /// 
    /// Calling the Read() or Write() methods with less than the maximum allowed
    /// number of bytes per time interval, but in total with more than that maximum
    /// number, the stream still throttles.
    /// </summary>
    public sealed class ThrottledStream : Stream
    {

        private readonly Stream _baseStream;
        private readonly bool _throttleReads;
        private readonly bool _throttleWrites;

        private readonly ThrottledOperation _throttledReadOperation;
        private readonly ThrottledOperation _throttledWriteOperation;

        /// <summary>
        /// Inits a new instance of a ThrottledStream with the base-stream to use as
        /// data source. The bytes-per-second sets the speed to which this stream-wrapper
        /// will throttle the calls to the base-stream, if necessary.
        /// </summary>
        /// <param name="baseStream">The base stream which is wrapped by this instance</param>
        /// <param name="bytesPerSecond">The speed to which the stream throttles</param>
        /// <param name="throttleReads">Set this true, if read-operations are to be throttled</param>
        /// <param name="throttleWrites">Set this true, if write-operations are to be throttled</param>
        /// <param name="intervalLength">The interval-length used to partition time measurements and sleep-operations.</param>
        public ThrottledStream(Stream baseStream, int bytesPerSecond, bool throttleReads = true, bool throttleWrites = false, int intervalLength = 1000)
        {
            _baseStream = baseStream;
            _throttleReads = throttleReads;
            _throttleWrites = throttleWrites;

            int bytesPerInterval = bytesPerSecond * intervalLength / 1000;

            _throttledReadOperation = new ThrottledOperation(
                intervalLength,
                bytesPerInterval,
                _baseStream.Read,
                (loadedChunkSize, bytesPerInterval) => loadedChunkSize < bytesPerInterval);

            _throttledWriteOperation = new ThrottledOperation(
                intervalLength,
                bytesPerInterval,
                (buffer, offset, count) =>
                {
                    _baseStream.Write(buffer, offset, count);
                    return bytesPerInterval;
                },
                (loadedChunkSize, bytesPerInterval) => false);
        }

        public override bool CanRead => _baseStream.CanRead;

        public override bool CanSeek => _baseStream.CanSeek;

        public override bool CanWrite => _baseStream.CanWrite;

        public override void Flush()
        {
            _baseStream.Flush();
        }

        public override long Length => _baseStream.Length;

        public override long Position { get => _baseStream.Position; set => _baseStream.Position = value; }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (_throttleReads)
            {
                return _throttledReadOperation.Throttle(buffer, offset, count);
            }
            else
            {
                return _baseStream.Read(buffer, offset, count);
            }
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
            if (_throttleWrites)
            {
                _throttledWriteOperation.Throttle(buffer, offset, count);
            }
            else
            {
                _baseStream.Write(buffer, offset, count);
            }
        }

    }
}
