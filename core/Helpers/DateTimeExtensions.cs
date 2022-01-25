using System;

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
    }
}