using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NodeMessaging;
using Utility.Core;

namespace UITemplateViewer.DynamicPath
{
    /// <summary>
    /// Instantiate a class based on an element definition
    /// </summary>
    public class PathContainer
    {
        private readonly IEnumerable<Assembly> _assemblies;

        public PathContainer(params Assembly[] assemblies)
        {
            _assemblies = assemblies;
        }

        public T CreateObject<T>(IParentNode templateNode, INode dataNode)
        {
            var creator = GetObjectCreator(_assemblies, templateNode);
            return (T) creator(dataNode);
        }

        //TODO: Call this recursively for all of the template nodes, and put it in a data structure for simpler retrieval later
        private Func<INode, object> GetObjectCreator(IEnumerable<Assembly> assemblies, IParentNode templateNode)
        {
            var predicate = BuildFilterFromElement(templateNode);
            var type = GetElementType(assemblies, predicate);
            var constructor = type.GetConstructors().Single();
            var setter = GetPropertiesSetter(type, templateNode);

            return node =>
                       {
                           var newObj = constructor.Invoke(new object[] {});
                           setter(newObj, node);
                           return newObj;
                       };
        }

        private Action<object, INode> GetPropertiesSetter(Type type, IParentNode templateNode)
        {
            var propertySetters = templateNode.Attributes().Select(attribute => GetPropertySetter(type, attribute)).ToArray();
            return (o, node) => propertySetters.Do(setter => setter(o, node));
        }

        private Action<object, INode> GetPropertySetter(Type type, INode attribute)
        {
            var propToSet = type
                .GetProperties()
                .SingleOrDefault(prop => prop.Name.ToLower() == attribute.Name.ToLower());

            if (propToSet == null) return (o, node) => { };

            var pathObjectFactory = new PathObjectFactory(CreateObject<object>, attribute, propToSet.PropertyType);
            var factory = pathObjectFactory.CreateFactory();

            return (o, node) => propToSet.SetValue(o, factory(node), null);
        }

        private static Predicate<Type> BuildFilterFromElement(INode templateNode)
        {
            return type => type.Name.ToLower() == templateNode.Name.ToLower()
                           && type.GetConstructors().Count() == 1;
        }

        private static Type GetElementType(IEnumerable<Assembly> assemblies, Predicate<Type> typeFilter)
        {
            try
            {
                return assemblies
                    .Select(assembly => assembly.GetTypes().SingleOrDefault(type => typeFilter(type)))
                    .Where(type => type != null)
                    .Single();
            }
            catch(Exception e)
            {
                var message = assemblies
                    .Select(assembly => assembly.GetTypes().Where(type => typeFilter(type)))
                    .Flatten()
                    .Select(type => type.Name)
                    .SeparateBy("\r\n");

                throw new Exception(message, e);
            }
        }
    }
}