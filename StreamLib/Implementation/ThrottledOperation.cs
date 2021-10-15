using System;
using System.Linq;
using System.Threading;

namespace StreamLib.Implementation
{
    internal class ThrottledOperation
    {

        private readonly Func<byte[], int, int, int> _operationFn;
        private readonly Func<int, int, bool> _exitConditionFn;

        private readonly Timer _timer;

        private readonly int _intervalLength = 1000; // interval length in milliseconds
        private readonly int _bytesPerInterval;
        private int _bytesProcessedInInterval = 0;

        internal ThrottledOperation(int intervalLength, int bytesPerInterval, Func<byte[], int, int, int> operationFn, Func<int, int, bool> exitConditionFn)
        {
            _timer = new Timer();

            _intervalLength = intervalLength;
            _bytesPerInterval = bytesPerInterval;

            _operationFn = operationFn;
            _exitConditionFn = exitConditionFn;
        }

        internal int Throttle(byte[] buffer, int offset, int count)
        {
            int totalBytes = 0;

            while (totalBytes < count)
            {
                int requestedChunkSize = Minimum(Throttle(), count, count - totalBytes);

                int loadedChunkSize = _operationFn(buffer, offset + totalBytes, requestedChunkSize);
                totalBytes += loadedChunkSize;

                _bytesProcessedInInterval += loadedChunkSize;
                if (_exitConditionFn(_bytesProcessedInInterval, _bytesPerInterval))
                {
                    return totalBytes;
                }
            }

            return totalBytes;
        }

        private int Throttle()
        {
            long millisecondsElapsedSinceLastRead = _timer.ElapsedMilliseconds;

            if (millisecondsElapsedSinceLastRead > _intervalLength)
            {
                return ResetTimeFrame();
            }

            int openCapacityInInterval = _bytesPerInterval - _bytesProcessedInInterval;
            if (openCapacityInInterval > 0)
            {
                return openCapacityInInterval;
            }

            // sleep for the rest of the base-ticks interval
            Thread.Sleep((int)(_intervalLength - millisecondsElapsedSinceLastRead));

            return ResetTimeFrame();
        }

        private int ResetTimeFrame()
        {
            _timer.Reset();
            _bytesProcessedInInterval = 0;
            return _bytesPerInterval;
        }

        private int Minimum(params int[] values)
        {
            return values.Min();
        }

    }
}
