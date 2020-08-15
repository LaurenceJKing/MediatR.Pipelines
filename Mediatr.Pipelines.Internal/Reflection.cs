using System;
using System.Linq;
using System.Reflection;

namespace Mediatr.Pipelines.Internal
{
    public static class Reflection
    {
        public static bool TryGetAttribute<T>(
            this ICustomAttributeProvider provider,
            out T attribute)
            where T : Attribute
        {
            attribute = provider
                .GetCustomAttributes(typeof(T), true)
                .Cast<T>()
                .SingleOrDefault();

            return attribute != null;
        }
    }
}
