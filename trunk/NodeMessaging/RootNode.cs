using System;
using System.Collections.Generic;
using System.Linq;
using ActionDictionary;
using Castle.Core.Interceptor;
using Utility.Core;

namespace NodeMessaging
{
    public class RootNode : IParentNode, IEndNode
    {
        private readonly List<NodeMessage> _nodeMessages = new List<NodeMessage>();
        private readonly Dictionary<Type, object> _registeredTypes = new Dictionary<Type, object>();

        public IEnumerable<IParentNode> Nodes(string nameFilter)
        {
            if (_registeredTypes.ContainsKey(typeof(IParentNode)))
            {
                var nodeHandler = (IParentNode)_registeredTypes[typeof (IParentNode)];
                return nodeHandler.Nodes(nameFilter).Select(node => (IParentNode)new ParentNodeWrapper(this, node));
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
            throw new System.NotImplementedException();
        }

        public void Register<T>(T t)
        {
            _registeredTypes[typeof (T)] = t;
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

        public void InstallHook(NodeMessage nodeMessage)
        {
            _nodeMessages.Add(nodeMessage);
        }

        public void Intercept(INode node, IInvocation invocation)
        {
            _nodeMessages.Do(message => ProcessMessage(message, node, invocation));
        }

        private void ProcessMessage(NodeMessage message, INode node, IInvocation invocation)
        {
            if (message.NodePredicate != null)
                if (!message.NodePredicate(node)) return;
            if (message.MessagePredicate != null)
                if (!message.MessagePredicate(invocation)) return;

            var lambda = invocation.Method.BuildLambda(invocation.Arguments);
            var msgOut = Message.Create(lambda, invocation.Method.DeclaringType);
            msgOut.Invoke(message.Target);
        }
    }
}