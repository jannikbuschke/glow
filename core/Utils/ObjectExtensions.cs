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
    }
}