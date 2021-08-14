using System;

namespace StreamLib.Implementation
{
    /// <summary>
    /// The Timer class helps measuring elapsed milliseconds
    /// since its last reset.
    /// </summary>
    internal class Timer
    {

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
        /// Gets the number of ticks between the last reset and
        /// the current value of DateTime.Now.Ticks, scaled to
        /// milliseconds.
        /// </summary>
        /// <returns></returns>
        internal long GetMilliseconds()
        {
            return (DateTime.Now.Ticks - _ticks) / TimeSpan.TicksPerMillisecond;
        }

    }
}
