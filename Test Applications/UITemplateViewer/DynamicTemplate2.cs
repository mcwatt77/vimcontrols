using System.Linq;
using System.Xml.Linq;
using NodeMessaging;
using UITemplateViewer.DynamicPath;
using UITemplateViewer.Element;

namespace UITemplateViewer
{
    public class DynamicTemplate2
    {
        private IParentNode dataNode;
        private IParentNode templateNode;

        public DynamicTemplate2()
        {
            Setup();
        }

        public void Setup()
        {
            var rootNode = new RootNode();
            var dataXml = XmlNode.Parse("<data><note desc=\"1\" body=\"One!\"/><note desc=\"2\" body=\"Two?\"/></data>");
            var template = XDocument.Load(@"..\..\templates\noteviewer.xml");
            var templateXml = XmlNode.Parse(template.ToString());
            rootNode.Register<IParentNodeImplementor>(new AggregateNode(dataXml, templateXml));
            dataNode = rootNode.Nodes("data").First();
            templateNode = rootNode.Nodes().Skip(1).First();
        }

        public IUIInitialize GetUI()
        {
            var pathContainer = new PathContainer(typeof (IUIInitialize).Assembly);
            return pathContainer.CreateObject<IUIInitialize>(templateNode, dataNode);
        }
    }
}
