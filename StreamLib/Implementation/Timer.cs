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
        private const long TicksPerSecond = TimeSpan.TicksPerMillisecond * 1000;

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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Reset()
        {
            _ticks = DateTime.Now.Ticks;
        }

        /// <summary>
        /// Gets the elapsed time of this timer as a <see cref="TimeSpan"/>.
        /// </summary>
        internal TimeSpan ElapsedTime
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return TimeSpan.FromTicks(GetTicksElapsed());
            }
        }

        /// <summary>
        /// Gets the number elapsed milliseconds between the last reset and
        /// the current value of DateTime.Now.Ticks, scaled to
        /// milliseconds.
        /// </summary>
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
        internal long ElapsedNanoseconds
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return GetTicksElapsed() / TicksPerNanoSecond;
            }
        }

        /// <summary>
        /// Gets the number of seconds elapsed since the last reset
        /// of this timer.
        /// </summary>
        internal long ElapsedSeconds
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return GetTicksElapsed() / TicksPerSecond;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private long GetTicksElapsed()
        {
            return DateTime.Now.Ticks - _ticks;
        }

    }
}
