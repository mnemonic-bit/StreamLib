using System;

namespace StreamLib
{
    public class MeteringEventArgs
    {

        public MeteringEventArgs(TimeSpan timeElapsed, int totelBytes, long averageSpeed, long currentSpeed)
        {
            TimeElapsed = timeElapsed;
            TotelBytes = totelBytes;
            AverageSpeed = averageSpeed;
            CurrentSpeed = currentSpeed;
        }

        public TimeSpan TimeElapsed { get; }
        public int TotelBytes { get; }
        /// <summary>
        /// Gets the average speed in bytes per second.
        /// </summary>
        public long AverageSpeed { get; }
        /// <summary>
        /// Gets the current speed in bytes per second.
        /// </summary>
        public long CurrentSpeed { get; }

    }
}
