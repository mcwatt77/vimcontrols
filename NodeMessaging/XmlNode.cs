using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace NodeMessaging
{
    public class XmlNode : IParentNode
    {
        private readonly XElement _elem;

        private XmlNode(XElement elem)
        {
            _elem = elem;
        }

        public static XmlNode Parse(string xml)
        {
            var doc = XDocument.Parse(xml);
            return new XmlNode(new XElement("root", doc.Root));
        }

        public IEnumerable<IParentNode> Nodes(string nameFilter)
        {
            return _elem.Elements(nameFilter).Select(elem => (IParentNode)new XmlNode(elem));
        }

        public IEnumerable<IParentNode> Nodes()
        {
            return _elem.Elements().Select(elem => (IParentNode) new XmlNode(elem));
        }

        public IParentNode NodeAt(int index)
        {
            throw new System.NotImplementedException();
        }

        public IEndNode Attribute(string name)
        {
            var attr = _elem.Attribute(name);

            var endNode = new EndNode {Name = name, Parent = this};
            var stringProvider = (IFieldAccessor<string>)new FieldAccessor<string> {Value = attr.Value};
            endNode.Register(stringProvider);
            return endNode;
        }

        public IEnumerable<IEndNode> Attributes()
        {
            return _elem.Attributes().Select(attr => Attribute(attr.Name.LocalName));
        }

        public T Get<T>() where T : class
        {
            throw new System.NotImplementedException();
        }

        public void Register<T>(T t)
        {
            throw new System.NotImplementedException();
        }

        public IParentNode Parent
        {
            get { throw new System.NotImplementedException(); }
        }

        public string Name
        {
            get { return _elem.Name.LocalName; }
        }
    }
}
