namespace JannikB.Glue.AspNetCore.Clock
{
    using System;

    namespace TimeR.Clock
    {
        public interface IClock
        {
            DateTime UtcNow();
        }

        public class Clock : IClock
        {
            public DateTime UtcNow()
            {
                return DateTime.UtcNow;
            }
        }

        public class FakeClock : IClock
        {
            public FakeClock()
            {
                dateTime = DateTime.UtcNow;
            }

            private DateTime dateTime;

            public DateTime UtcNow()
            {
                return dateTime;
            }

            public void Set(DateTime dateTime)
            {
                this.dateTime = dateTime;
            }

            public DateTime Add(TimeSpan offset)
            {
                Set(UtcNow() + offset);
                return UtcNow();
            }
        }
    }
}
