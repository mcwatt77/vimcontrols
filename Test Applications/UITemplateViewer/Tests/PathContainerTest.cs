using System.Linq;
using System.Xml.Linq;
using NodeMessaging;
using NUnit.Framework;
using UITemplateViewer.DynamicPath;
using UITemplateViewer.Element;

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
            var ui = pathContainer.GetObject<IUIInitialize>(templateNode, dataNode);
        }
    }
}