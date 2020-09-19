using System;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Glow.Core
{
    public static class StopwatchExtension
    {
        public static void LogElapsedTime(this Stopwatch stopWatch, string message, ILogger logger)
        {
            TimeSpan ts = stopWatch.Elapsed;

            // Format and display the TimeSpan value.
            var elapsedTime = string.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);

            logger.LogDebug("{elapsedTime}: {message}", elapsedTime, message);
        }
    }
}
