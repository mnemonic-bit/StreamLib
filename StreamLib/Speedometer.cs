using StreamLib.Implementation;
using System.Runtime.CompilerServices;

namespace StreamLib
{
    /// <summary>
    /// This class implements the Exponential Moving Average (EMA) for determining
    /// the current speed of the connection.
    /// </summary>
    internal class Speedometer
    {

        public Speedometer()
        {
            _totalTime = new Timer();
            _totalTime.Reset();

            _timeSinceLastMeasurement = new Timer();
            _timeSinceLastMeasurement.Reset();

            lastSpeed = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public MeteringEventArgs MeasureSpeed(int totalBytes)
        {
            var totalElapsedTime = _totalTime.ElapsedTime;
            var totakElapsedTimeInSeconds = _totalTime.ElapsedSeconds;

            var millisecondsSinceLastMeasurement = _timeSinceLastMeasurement.ElapsedMilliseconds;
            _timeSinceLastMeasurement.Reset();

            var averageSpeed = totakElapsedTimeInSeconds == 0 ? 0 : totalBytes / totakElapsedTimeInSeconds;

            var differenceOfBytesLoaded = totalBytes - lastTotalBytes;
            var currentSpeed = millisecondsSinceLastMeasurement == 0 ? differenceOfBytesLoaded : differenceOfBytesLoaded * 1000 / millisecondsSinceLastMeasurement;
            currentSpeed = (int)(SMOOTHING_FACTOR * lastSpeed + (1 - SMOOTHING_FACTOR) * currentSpeed);
            lastSpeed = currentSpeed;
            lastTotalBytes = totalBytes;

            return new MeteringEventArgs(totalElapsedTime, totalBytes, averageSpeed, currentSpeed);
        }


        private readonly Timer _totalTime;
        private readonly Timer _timeSinceLastMeasurement;

        private int lastTotalBytes;
        private long lastSpeed;

        private const double SMOOTHING_FACTOR = 0.5d;

    }
}
