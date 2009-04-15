using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace NodeMessaging
{
    public class XmlNode : IParentNodeImplementor
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

        public IEnumerable<IParentNodeImplementor> Nodes(string nameFilter)
        {
            return _elem.Elements(nameFilter).Select(elem => (IParentNodeImplementor)new XmlNode(elem));
        }

        public IEnumerable<IParentNodeImplementor> Nodes()
        {
            return _elem.Elements().Select(elem => (IParentNodeImplementor) new XmlNode(elem));
        }

        public IParentNodeImplementor NodeAt(int index)
        {
            throw new System.NotImplementedException();
        }

        public IEndNodeImplementor Attribute(string name)
        {
            var attr = _elem.Attribute(name);

            var endNode = new EndNode {Name = name, Parent = this, Value = attr.Value};
            var stringProvider = (IAccessor<string>)new Accessor<string> {Value = attr.Value};
            endNode.Register(stringProvider);
            return endNode;
        }

        public IEnumerable<IEndNodeImplementor> Attributes()
        {
            return _elem.Attributes().Select(attr => Attribute(attr.Name.LocalName));
        }

        public IParentNodeImplementor Parent
        {
            get { throw new System.NotImplementedException(); }
        }

        public string Name
        {
            get { return _elem.Name.LocalName; }
        }
    }
}
