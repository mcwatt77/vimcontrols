using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using PathContainer.Node;

namespace PathContainer.NodeImplementor
{
    public class XmlImplementor : INodeImplementor, INodeCollection<INodeImplementor>, IDataNodeCollection
    {
        private readonly XElement _element;
        private readonly XAttribute _attribute;
        private IDataNode _attributeValue;

        public static XmlImplementor Parse(string xml)
        {
            return Parse(xml, null);
        }

        public static XmlImplementor Parse(string xml, string nameSpace)
        {
            var doc = XDocument.Parse(xml);
            if (doc.Root == null) throw new Exception("Xml Document must not have a missing root node");
            return new XmlImplementor(doc.Root);
        }

        private XmlImplementor(XElement element)
        {
            _element = element;
        }

        private XmlImplementor(XAttribute attribute)
        {
            _attribute = attribute;
            _attributeValue = GenericDataNode.Create(() => _attribute.Value);
        }

        public string Name
        {
            get
            {
                return _element != null 
                    ? _element.Name.LocalName 
                    : _attribute.Name.LocalName;
            }
            set
            {
                if (_element != null)
                    _element.Name = XName.Get(value, Namespace.ToString());
                else
                    throw new Exception("Setting Name of a node with an underlying type of XAttribute not allowed");
            }
        }

        public Namespace Namespace
        {
            get
            {
                return Namespace.Get(_element != null
                                         ? _element.Name.NamespaceName
                                         : _attribute.Name.NamespaceName);
            }
            set
            {
                if (_element != null)
                    _element.Name = XName.Get(Name, value.ToString());
                else
                    throw new Exception("Setting Name of a node with an underlying type of XAttribute not allowed");
            }
        }

        private static readonly EmptyNodeCollection _emptyNodeCollection = new EmptyNodeCollection();
        private class EmptyNodeCollection : INodeCollection<INodeImplementor>
        {
            public INodeImplementor GetByName(string name)
            {
                return null;
            }

            public IEnumerator<INodeImplementor> GetEnumerator()
            {
                yield break;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        public INodeCollection<INodeImplementor> Nodes
        {
            get
            {
                return _element != null
                           ? this
                           : (INodeCollection<INodeImplementor>) _emptyNodeCollection;
            }
        }

        private static readonly EmptyDataNodeCollection _emptyDataNodeCollection = new EmptyDataNodeCollection();
        private class EmptyDataNodeCollection : IDataNodeCollection
        {
            public IEnumerator<KeyValuePair<string, IDataNode>> GetEnumerator()
            {
                yield break;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public IDataNode GetBySlotName(string slotName)
            {
                return null;
            }

            public IDataNode Register(string slotName, Func<object> fnNew)
            {
                return null;
            }
        }

        public IDataNodeCollection DataNodes
        {
            get
            {
                return _attribute != null
                           ? this
                           : (IDataNodeCollection)_emptyDataNodeCollection;
            }
        }

        IEnumerator<KeyValuePair<string, IDataNode>> IEnumerable<KeyValuePair<string, IDataNode>>.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public IEnumerator<INodeImplementor> GetEnumerator()
        {
            var elements = _element
                .Elements()
                .Select(element => (INodeImplementor)new XmlImplementor(element));

            var attributes = _element
                .Attributes()
                .Select(attribute => (INodeImplementor)new XmlImplementor(attribute));

            return attributes
                .Concat(elements)
                .GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public INodeImplementor GetByName(string name)
        {
            var element = _element.Element(XName.Get(name, _element.Name.NamespaceName));
            if (element != null) return new XmlImplementor(element);
            var attribute = _element.Attribute(XName.Get(name, _element.Name.NamespaceName));
            if (attribute != null) return new XmlImplementor(attribute);
            return null;
        }

        public IDataNode GetBySlotName(string slotName)
        {
            if (slotName != Slot.StringData) throw new Exception("XmlNodes only support " + Slot.StringData);

            return _attributeValue;
        }

        public IDataNode Register(string slotName, Func<object> fnNew)
        {
            if (slotName != Slot.StringData) throw new Exception("XmlNodes only support " + Slot.StringData);

            return _attributeValue = GenericDataNode.Create(fnNew);
        }
    }
}