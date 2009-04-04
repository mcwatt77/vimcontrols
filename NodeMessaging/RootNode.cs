using System;
using System.Collections.Generic;
using System.Linq;
using ActionDictionary;
using Castle.Core.Interceptor;
using Utility.Core;

namespace NodeMessaging
{
    public class RootNode : NodeBase, IParentNode, IEndNode
    {
        private readonly List<NodeMessage> _nodeMessages = new List<NodeMessage>();
        private List<ParentNodeWrapper> _parentNodeWrappers;
        private readonly Dictionary<string, IEnumerable<IParentNode>> _nodeDict = new Dictionary<string, IEnumerable<IParentNode>>();

        public IEnumerable<IParentNode> Nodes(string nameFilter)
        {
            if (_registeredTypes.ContainsKey(typeof(IParentNodeImplementor)))
            {
                    var nodeHandler = (IParentNodeImplementor) _registeredTypes[typeof (IParentNodeImplementor)];
                    if (!_nodeDict.ContainsKey(nameFilter))
                    {
                        _nodeDict[nameFilter] =
                            nodeHandler.Nodes(nameFilter).Select(node => (IParentNode)new ParentNodeWrapper(this, node)).ToList();
                    }
                    return _nodeDict[nameFilter];
            }
            throw new Exception("You did not register an IParentNode handler");
        }

        public IEnumerable<IParentNode> Nodes()
        {
            if (_registeredTypes.ContainsKey(typeof(IParentNodeImplementor)))
            {
                if (_parentNodeWrappers == null)
                {
                    var nodeHandler = (IParentNodeImplementor) _registeredTypes[typeof (IParentNodeImplementor)];
                    _parentNodeWrappers =
                        nodeHandler.Nodes().Select(node => new ParentNodeWrapper(this, node)).ToList();
                }
                return _parentNodeWrappers.Cast<IParentNode>();
            }
            throw new Exception("You did not register an IParentNode handler");
        }

        public IParentNode NodeAt(int index)
        {
            throw new System.NotImplementedException();
        }

        public IEndNode Attribute(string name)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<IEndNode> Attributes()
        {
            throw new System.NotImplementedException();
        }

        public IParentNode Parent
        {
            get { throw new System.NotImplementedException(); }
        }

        public string Name
        {
            get { throw new System.NotImplementedException(); }
        }

        public T Get<T>() where T : class
        {
            return (T) _registeredTypes[typeof (T)];
        }

        public IParentNode Root
        {
            get { return this; }
        }

        public Message Send(Message message)
        {
            //how do I know who to send to?
            return null;
        }

        public IEnumerable<Type> RegisteredTypes
        {
            get { throw new System.NotImplementedException(); }
        }

        public void InstallHook<T>(T tHook, object recipient)
        {
            throw new System.NotImplementedException();
        }

        public void InstallHook(NodeMessage nodeMessage)
        {
            _nodeMessages.Add(nodeMessage);
        }

        public void Intercept(INodeImplementor node, IInvocation invocation)
        {
            _nodeMessages.Do(message => ProcessMessage(message, node, invocation));
        }

        private void ProcessMessage(NodeMessage message, INodeImplementor node, IInvocation invocation)
        {
            if (invocation.Method.Name == "set_SelectedRow")
            {
                int debug = 0;
            }
            INode nodeCmp = null;
            if (typeof(IParentNodeImplementor).IsAssignableFrom(node.GetType()))
                nodeCmp = new ParentNodeWrapper(this, (IParentNodeImplementor)node);
            if (typeof(IEndNodeImplementor).IsAssignableFrom(node.GetType()))
                nodeCmp = new EndNodeWrapper(this, (IEndNodeImplementor)node);

            if (message.NodePredicate != null)
                if (!message.NodePredicate(nodeCmp)) return;
            if (message.MessagePredicate != null)
                if (!message.MessagePredicate(invocation)) return;

            var del = message.TargetDelegate;
            var action = del.GetType().GetMethod("Invoke").Invoke(del, new object[] {invocation});
            action.GetType().GetMethod("Invoke").Invoke(action, new [] {message.Target});
        }
    }
}