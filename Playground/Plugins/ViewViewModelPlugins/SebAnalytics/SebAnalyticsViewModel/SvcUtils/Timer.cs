using System;

namespace Sebastion.Core.SvcUtils
{
    public class Timer
    {
        private DateTime _startTime;

        public TimeSpan Interval { get; private set; }

        public Timer()
        {
            ResetStartTime();
        }

        public void SetInterval()
        {
            Interval = DateTime.Now.Subtract(_startTime);

            ResetStartTime();
        }

        private void ResetStartTime()
        {
            _startTime = DateTime.Now;
        }
    }
}
