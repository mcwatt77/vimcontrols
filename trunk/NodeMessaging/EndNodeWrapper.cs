using System;
using System.Collections.Generic;
using ActionDictionary;

namespace NodeMessaging
{
    public class EndNodeWrapper : NodeBase, IEndNode
    {
        private readonly IParentNode _parent;

        public EndNodeWrapper(RootNode rootNode, INodeImplementor node, IParentNode parent) : base(rootNode, node)
        {
            _parent = parent;
        }

        public string Name
        {
            get { return _node.Name; }
        }

        public IParentNode Parent
        {
            get { return _parent; }
        }

        public IParentNode Root
        {
            get { return _rootNode; }
        }

        public Message Send(Message message)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<Type> RegisteredTypes
        {
            get { throw new System.NotImplementedException(); }
        }

        public void InstallHook<T>(T tHook, object recipient)
        {
            throw new System.NotImplementedException();
        }
    }
}