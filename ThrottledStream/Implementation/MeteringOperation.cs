using System;
using System.Linq;

namespace StreamLib.Implementation
{
    internal class MeteringOperation
    {

        private readonly Func<byte[], int, int, int> _operationFn;
        private readonly Func<int, int, bool> _exitConditionFn;

        private readonly int _chunkSize;

        internal MeteringOperation(int intervalLength, Func<byte[], int, int, int> operationFn, Func<int, int, bool> exitConditionFn)
        {
            _operationFn = operationFn;
            _exitConditionFn = exitConditionFn;

            // We assume here that 1k per second is a speed to start with,
            // and set the chunk size accordingly.
            _chunkSize = 1024 * 1000 / intervalLength;
        }

        /// <summary>
        /// Event handler which receives the on-progress events
        /// created by this component.
        /// </summary>
        public event Action<int> OnProgress;

        internal int MeterOperation(byte[] buffer, int offset, int count)
        {
            int totalBytes = 0;

            while (totalBytes < count)
            {
                int requestedChunkSize = Minimum(_chunkSize, count, count - totalBytes);

                int loadedChunkSize = _operationFn(buffer, offset + totalBytes, requestedChunkSize);
                totalBytes += loadedChunkSize;

                OnProgress(totalBytes);

                if (_exitConditionFn(loadedChunkSize, totalBytes))
                {
                    return totalBytes;
                }
            }

            return totalBytes;
        }

        private int Minimum(params int[] values)
        {
            return values.Min();
        }

    }
}
