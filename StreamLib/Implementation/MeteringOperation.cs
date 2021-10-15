using System;
using System.Linq;

namespace StreamLib.Implementation
{
    internal class MeteringOperation
    {

        private readonly Func<byte[], int, int, int> _operationFn;
        private readonly Func<int, int, bool> _exitConditionFn;

        private readonly Timer _timer;
        private readonly Timer _operationTimer;
        private readonly int _intervalLengthInNanoseconds;
        private int _chunkSize;

        internal MeteringOperation(int intervalLength, Func<byte[], int, int, int> operationFn, Func<int, int, bool> exitConditionFn)
        {
            _intervalLengthInNanoseconds = intervalLength * 1000;
            _operationFn = operationFn;
            _exitConditionFn = exitConditionFn;

            _timer = new Timer();
            _operationTimer = new Timer();

            // We assume here that 1k per second is a speed to start with,
            // and set the chunk size accordingly.
            _chunkSize = 1024 * 1000 / intervalLength;
        }

        /// <summary>
        /// Event handler which receives the on-progress events
        /// created by this component.
        /// </summary>
        public event Action<TimeSpan, int> OnProgress;

        internal int MeterOperation(byte[] buffer, int offset, int count)
        {
            int totalBytes = 0;

            while (totalBytes < count)
            {
                int requestedChunkSize = Minimum(_chunkSize, count, count - totalBytes);

                _operationTimer.Reset();

                int loadedChunkSize = _operationFn(buffer, offset + totalBytes, requestedChunkSize);

                long elapsedNanoseconds = _operationTimer.ElapsedNanoseconds;
                AdaptChunkSizeToActualSpeed(elapsedNanoseconds);

                totalBytes += loadedChunkSize;

                OnProgress(_timer.ElapsedTime, totalBytes);

                if (_exitConditionFn(loadedChunkSize, totalBytes))
                {
                    return totalBytes;
                }
            }

            return totalBytes;
        }

        private void AdaptChunkSizeToActualSpeed(long elapsedNanoseconds)
        {
            double speedDivertionRatio = (double)_intervalLengthInNanoseconds / (double)(elapsedNanoseconds + 1);
            if (Math.Abs(speedDivertionRatio - 1d) > 1d)
            {
                _chunkSize = (int)((double)_chunkSize * speedDivertionRatio / 100d);
            }
        }

        private int Minimum(params int[] values)
        {
            return values.Min();
        }

    }
}
