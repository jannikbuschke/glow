using System;

namespace Glow.Clocks
{
    public interface IClock
    {
        /// <summary>
        /// Returns DateTime.UtcNow
        /// </summary>
        [Obsolete("Use IClock.Now")]
        DateTime UtcNow();

        /// <summary>
        /// Returns DateTime.UtcNow
        /// </summary>
        DateTime Now { get; }
    }

    public class Clock : IClock
    {
        public DateTime Now
        {
            get
            {
                return DateTime.UtcNow;
            }
        }

        public DateTime UtcNow()
        {
            return DateTime.UtcNow;
        }
    }

    public class FakeClock : IClock
    {
        public FakeClock()
        {
            Now = DateTime.UtcNow;
        }

        public DateTime Now { get; private set; }

        public DateTime UtcNow()
        {
            return Now;
        }

        public void Set(DateTime dateTime)
        {
            Now = dateTime;
        }

        public DateTime Add(TimeSpan offset)
        {
            Set(UtcNow() + offset);
            return UtcNow();
        }
    }
}