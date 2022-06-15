using System;
using NodaTime;
using NodaTime.Extensions;

namespace Glow.Core.Helpers
{
    public static class DateTimeExtensions
    {
        public static string ToIso8601String(this DateTime dateTime)
        {
            return dateTime.ToString("o");
        }

        public static string ToIso8601String(this DateTimeOffset  dateTime)
        {
            return dateTime.ToString("o");
        }

        public static Instant FromUnspecifiedToUtcInstant(this DateTime dateTime)
        {
            var utc = DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
            return utc.ToInstant();
        }
    }
}