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

        public static IEnumerable<T> Descendants<T>(this T e, Func<T, IEnumerable<T>> fn) where T : class
        {
            var existingList = new HashSet<T>();
            Descendants(existingList, e, fn);
            return existingList.Where(type => type != e);
        }

	    private static void Descendants<T>(HashSet<T> existingList, T e, Func<T, IEnumerable<T>> fn)
	    {
            if (existingList.Contains(e)) return;

	        existingList.Add(e);
	        var newE = fn(e);
	        if (newE == null) return;

            foreach (var item in newE)
                Descendants(existingList, item, fn);
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