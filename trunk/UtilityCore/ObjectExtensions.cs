using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Utility.Core
{
	public static class ObjectExtensions
	{
        public static IEnumerable<T> Chain<T>(this T e, Func<T, T> fn) where T : class
        {
            var newE = fn(e);
            while (newE != null)
            {
                yield return newE;
                newE = fn(newE);
            }
        }

        public static IEnumerable<T> ChainWithSelf<T>(this T e, Func<T, T> fn) where T : class
        {
            yield return e;
            var newE = fn(e);
            while (newE != null)
            {
                yield return newE;
                newE = fn(newE);
            }
        }

		public static PropertyInfo Property(this object instance, string propertyName)
		{
			if (instance == null)
				throw new ArgumentNullException("instance");

			var propInfo = instance.GetType()
				.GetProperties()
				.Where(prop => prop.Name.ToLower() == propertyName.ToLower())
				.SingleOrDefault();

			if (propInfo == null)
				throw new InvalidOperationException(propertyName + " is not valid for type " + instance.GetType());

			return propInfo;
		}

		public static T PropertyValue<T>(this object instance, string propertyName)
		{
			if (instance == null)
				throw new ArgumentNullException("instance");

			var propInfo = instance.GetType().GetProperty(propertyName);

			if (propInfo == null)
				throw new InvalidOperationException(propertyName + " is not valid for type " + instance.GetType());

			var val = propInfo.GetValue(instance, null);
			return (T)val;
		}
	}
}