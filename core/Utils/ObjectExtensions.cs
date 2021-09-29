using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Glow.Core.Utils
{
    public static class GetMethodNameObjectExtension
    {
        public static string GetMethodName(this object type, [CallerMemberName] string caller = null,
            bool getFullName = false)
        {
            return getFullName
                ? type.GetType().FullName + "." + caller
                : caller;
        }

        public static Dictionary<string, object> ToDictionary(this object any)
        {
            var dict = any.GetType()
                .GetProperties()
                .ToDictionary(x => x.Name, x => x.GetValue(any, null));
            return dict;
        }
    }
}