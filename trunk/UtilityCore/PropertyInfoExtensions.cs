using System.Linq;
using System.Reflection;

namespace Utility.Core
{
    public static class PropertyInfoExtensions
    {
        public static bool HasGetterSetter(this PropertyInfo propertyInfo)
        {
            var accessors = propertyInfo.GetAccessors();
            if (accessors == null) return false;
            return accessors.Count() == 2;
        }
    }
}
