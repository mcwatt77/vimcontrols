using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Utility.Core
{
    public static class CustomAttributeProviderExtensions
    {
        public static IEnumerable<TAttributeType> AttributesOfType<TAttributeType>(this ICustomAttributeProvider attr)
        {
            return attr
                .GetCustomAttributes(false)
                .Where(o => typeof(TAttributeType).IsAssignableFrom(o.GetType()))
                .Cast<TAttributeType>();
        }
    }
}
