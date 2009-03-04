using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;

namespace Utility.Core
{
    public static class TypeExtensions
    {
        public static IEnumerable<Type> GetImplementations(this Type interfaceType, Assembly assembly)
        {
            return assembly
                .GetTypes()
                .Where(
                type => (interfaceType.IsAssignableFrom(type) && !type.IsAbstract));
        }

        public static IEnumerable<Type> GetImplementations(this Type interfaceType)
        {
            return interfaceType.GetImplementations(typeof (TypeExtensions).Assembly);
        }

        public static string GetClassShortName(this Type type)
        {
            return type.Name.Split('.').Last();
        }

        public static string FullNameAndAssembly(this Type type)
        {
            return type.FullName + ", " + type.Assembly.GetName().Name;
        }

        public static XDocument LoadXmlResource(this Type type, string fileName)
        {
            var nameSpace = type.FullName.Replace(type.Name, String.Empty);
            var resourceName = nameSpace + fileName + ".xml";
            var stream = type
                .Assembly
                .GetManifestResourceStream(resourceName);

            if (stream == null) return null;

            var textReader = new XmlTextReader(stream);
            return XDocument.Load(textReader);
        }

        public static TItem NewInstance<TItem>(this Type type)
        {
            return (TItem)type.GetConstructors().Single().Invoke(new object[] {});
        }
    }
}