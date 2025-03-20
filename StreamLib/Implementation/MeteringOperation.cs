using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace StreamLib.Implementation
{
    internal sealed class MeteringOperation
    {

        /// <summary>
        /// Inits a metering operatin. The length of the measurement interval
        /// can be set with a parameter, progress-updates are given approximately
        /// once in the interval.
        /// </summary>
        /// <param name="intervalLength">The length of the measurement interval.</param>
        /// <param name="operationFn"></param>
        /// <param name="exitConditionFn"></param>
        internal MeteringOperation(int intervalLength, Func<byte[], int, int, int> operationFn, Func<int, int, bool> exitConditionFn)
        {
            _intervalLengthInNanoseconds = intervalLength * 1000;
            _operationFn = operationFn;
            _exitConditionFn = exitConditionFn;

            _operationTimer = new Timer();
            _meteringEventTimer = new Timer();
            _meteringEventTimer.Reset();

            _speedometer = new Speedometer();

            // We assume here that approx. 1k per second is a speed to start with,
            // and set the chunk size accordingly.
            _chunkSize = 10;// intervalLength;
        }

        /// <summary>
        /// Event handler which receives the on-progress events
        /// created by this component.
        /// </summary>
        internal event Action<MeteringEventArgs>? OnProgress;

        internal int MeterOperation(byte[] buffer, int offset, int count)
        {
            int totalBytesSent = 0;

            while (totalBytesSent < count)
            {
                int requestedChunkSize = Minimum(_chunkSize, count, count - totalBytesSent);

                _operationTimer.Reset();

                int loadedChunkSize = _operationFn(buffer, offset + totalBytesSent, requestedChunkSize);

                long elapsedNanoseconds = _operationTimer.ElapsedNanoseconds;
                AdaptChunkSizeToActualSpeed(elapsedNanoseconds);

                totalBytesSent += loadedChunkSize;

                SendMeteringEvent(totalBytesSent);

                if (_exitConditionFn(loadedChunkSize, requestedChunkSize))
                {
                    return totalBytesSent;
                }
            }

            return totalBytesSent;
        }


        private readonly Func<byte[], int, int, int> _operationFn;
        private readonly Func<int, int, bool> _exitConditionFn;

        private readonly Timer _operationTimer;
        private readonly Timer _meteringEventTimer;
        private readonly int _intervalLengthInNanoseconds;
        private int _chunkSize;
        private readonly long _meteringEventTimeThreshold = 500000;

        private readonly Speedometer _speedometer;


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AdaptChunkSizeToActualSpeed(long elapsedNanoseconds)
        {
            double speedDivertionRatio = ((double)_intervalLengthInNanoseconds + 1d) / (double)(elapsedNanoseconds + 1);
            double ratio = Math.Min(20, speedDivertionRatio);
            if (Math.Abs(ratio - 1d) > 0.1d)
            {
                _chunkSize = (int)((double)_chunkSize * ratio);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int Minimum(params int[] values)
        {
            return values.Min();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SendMeteringEvent(int totalBytesSent)
        {
            if (OnProgress == null)
            {
                return;
            }

            if (_meteringEventTimer.ElapsedNanoseconds < _meteringEventTimeThreshold)
            {
                return;
            }

            OnProgress(_speedometer.MeasureSpeed(totalBytesSent));

            _meteringEventTimer.Reset();
        }

    }
}
