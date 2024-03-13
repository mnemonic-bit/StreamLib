using System;
using System.Runtime.CompilerServices;

namespace StreamLib.Implementation
{
    /// <summary>
    /// The Timer class helps measuring elapsed milliseconds
    /// since its last reset.
    /// </summary>
    internal sealed class Timer
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
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return GetTicksElapsed() / TicksPerNanoSecond;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private long GetTicksElapsed()
        {
            return DateTime.Now.Ticks - _ticks;
        }

    }
}
