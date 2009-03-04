using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Utility.Core
{
    public static class AssemblyExtensions
    {
        public static TBase NewInstanceOfType<TBase>(this Assembly src, string namespacePart, string typeName, params object[] parms) where TBase : class
        {
            var newType = src.FindType<TBase>(typeName, namespacePart);

            if (parms.Where(o => o == null).Any())
                throw new ArgumentException("Can't initialize an object with null constructor values.");

            var constructor = newType.GetConstructor(parms.Select(o => o.GetType()).ToArray());
            if (constructor == null) throw new ArgumentException("Class does not have a matching constructor.");
            return (TBase)constructor.Invoke(parms);
        }

        private static Type FindType<TBase>(this Assembly src, string typeName, string namespacePart)
        {
            var newType = src.GetTypes()
                .Where(type => type.Name == typeName &&
                               typeof(TBase).IsAssignableFrom(type) &&
                               (namespacePart == null ? true : type.FullName.Contains(namespacePart)))
                .SingleOrDefault();
            if (newType == null) throw new ArgumentException("Class of type " + typeName + " not found.");
            return newType;
        }

        public static IEnumerable<MethodInfo> GetStaticMethodsWithCustomAttribute<T>(this Assembly assembly)
        {
            return assembly
                .GetTypes()
                .Select(type => type.GetMethods(BindingFlags.Public | BindingFlags.Static).AsEnumerable())
                .Flatten()
                .Where(method => method.AttributesOfType<T>().Any());
        }

        public static IEnumerable<Type> GetTypesWithCustomAttribute<T>(this Assembly assembly)
        {
            return assembly
                .GetTypes()
                .Where(type => type.AttributesOfType<T>().Any());
        }

        public static IEnumerable<MethodInfo> GetMethodsWithCustomAttribute<T>(this Assembly assembly)
        {
            return assembly
                .GetTypes()
                .Select(type => type.GetMethods().AsEnumerable())
                .Flatten()
                .Where(method => method.AttributesOfType<T>().Any());
        }
    }
}