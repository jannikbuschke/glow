using System.Collections.Generic;

namespace Glow.Core.Helpers
{
    public static class ListExtensions
    {
        public static void AddIfNotEmptyOrNull(this IList<string> self, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                self.Add(value);
            }
        }

        public static void AddIfNotNull<T>(this IList<T> self, T value)
        {
            if (value != null)
            {
                self.Add(value);
            }
        }
    }
}