using System;

namespace StreamLib.Implementation
{
    /// <summary>
    /// The Timer class helps measuring elapsed milliseconds
    /// since its last reset.
    /// </summary>
    internal class Timer
    {

        private const long TicksPerNanoSecond = TimeSpan.TicksPerMillisecond / 1000;

        private long _ticks = 0;

        /// <summary>
        /// Inits a new Timer with a ticks-cound of 0.
        /// </summary>
        internal Timer()
        {
        }

        /// <summary>
        /// Resets the ticks-cound to the current value of
        /// DateTime.Now.Ticks
        /// </summary>
        internal void Reset()
        {
            _ticks = DateTime.Now.Ticks;
        }

        /// <summary>
        /// Gets the elapsed time of this timer.
        /// </summary>
        internal TimeSpan ElapsedTime
        {
            get
            {
                return TimeSpan.FromTicks(_ticks);
            }
        }

        /// <summary>
        /// Gets the number elapsed milliseconds between the last reset and
        /// the current value of DateTime.Now.Ticks, scaled to
        /// milliseconds.
        /// </summary>
        /// <returns></returns>
        internal long ElapsedMilliseconds
        {
            get
            {
                return GetTicksElapsed() / TimeSpan.TicksPerMillisecond;
            }
        }

        /// <summary>
        /// Gets the number of nanoseconds elapsed since the last
        /// reset of this timer.
        /// </summary>
        /// <returns></returns>
        internal long ElapsedNanoseconds
        {
            get
            {
                return GetTicksElapsed() / TicksPerNanoSecond;
            }
        }

        private long GetTicksElapsed()
        {
            return DateTime.Now.Ticks - _ticks;
        }

    }
}
