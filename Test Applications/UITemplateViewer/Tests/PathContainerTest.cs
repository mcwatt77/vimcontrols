using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using NodeMessaging;
using NUnit.Framework;
using UITemplateViewer.DynamicPath;
using UITemplateViewer.Element;
using Utility.Core;

namespace UITemplateViewer.Tests
{
    [TestFixture]
    public class PathContainerTest
    {
        private IParentNode dataNode;
        private IParentNode templateNode;

        private void SetupRootNodes()
        {
            var rootNode = new RootNode();
            var dataXml = XmlNode.Parse("<data><note desc=\"1\" body=\"One!\"/><note desc=\"2\" body=\"Two?\"/></data>");
            var template = XDocument.Load(@"..\..\templates\noteviewer.xml");
            var templateXml = XmlNode.Parse(template.ToString());
            rootNode.Register<IParentNodeImplementor>(new AggregateNode(dataXml, templateXml));
            dataNode = rootNode.Nodes("data").First();
            templateNode = rootNode.Nodes().Skip(1).First();
        }

        [SetUp]
        public void Setup()
        {
            SetupRootNodes();
        }

        [Test]
        public void Test()
        {
            var pathContainer = new PathContainer(typeof (PathContainerTest).Assembly);
            var ui = pathContainer.CreateObject<IUIInitialize>(templateNode, dataNode);
            ui.Initialize();
        }
    }

    public class PathObjectFactory
    {
        private readonly Func<IParentNode, INode, object> fnNodeBuilder;
        private readonly INode _attribute;

        public PathObjectFactory(Func<IParentNode, INode, object> fnNodeBuilder, INode attribute)
        {
            this.fnNodeBuilder = fnNodeBuilder;
            _attribute = attribute;
        }

        public Func<INode, object> CreateFactory()
        {
            var get = _attribute.Get<IFieldAccessor<string>>();
            var path = Decoder.FromPath(get.Value);
            return dataNode =>
                       {
                           var fnLocal = path.Local.Compile();
                           var result = (IEnumerable<IParentNode>)fnLocal.DynamicInvoke(_attribute.Parent);
                           //TODO: instead of this it should actually be for each dataNode as well
                           var finalResult = result.Select(templateNode => fnNodeBuilder(templateNode, dataNode));
                           finalResult.ToList();
//                           fnNodeBuilder((IParentNode)result, dataNode);

                           //now create a new object

                           return result;
                       };
        }
    }

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

            var pathObjectFactory = new PathObjectFactory(CreateObject<object> ,attribute);
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