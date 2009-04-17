using System;
using System.Collections.Generic;
using PathContainer.NodeImplementor;

namespace PathContainer.Node
{
    public class RootNode : INode
    {
        private readonly Dictionary<string, INode> _implementorDictionary = new Dictionary<string, INode>();

        public string Name { get { return "root"; } }
        public Namespace NameSpace { get { return Namespace.Get("root"); } }
        public INode Parent { get { return null; } }
        public INode Root { get { return this; } }

        public INode RegisterNodeImplementor(string nameSpace, INodeImplementor implementor)
        {
            var node = new NodeWrapper(this, this, Namespace.Get(nameSpace), implementor);
            _implementorDictionary[nameSpace] = node;
            return node;
        }

        public INodeCollection Nodes
        {
            get
            {
//                var allNodes = _implementorDictionary.
//                return new NodeWrapperCollection()
                return null;
            }
        }

        public IDataNodeCollection DataNodes
        {
            get { throw new NotImplementedException(); }
        }

        //need to have special handling... a method to tell the collection what to do when requesting an INode

        private List<Func<string, INode, IDataNode, IDataNode>> _delegates = new List<Func<string, INode, IDataNode, IDataNode>>();

        public void RegisterNodeRequestDelegate(Func<string, INode, IDataNode, IDataNode> fnDelegate)
        {
            _delegates.Add(fnDelegate);
        }

        internal IDataNodeCollection WrapDataNodeCollection(NodeWrapper wrapper, IDataNodeCollection nodes)
        {
            return new DataNodeCollectionInjector(wrapper, _delegates, new DataNodeCollectionWrapper(nodes));
        }
    }
}