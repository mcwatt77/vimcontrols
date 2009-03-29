using System;
using System.Collections.Generic;
using System.Linq;
using ActionDictionary;
using Castle.Core.Interceptor;
using Castle.DynamicProxy;
using Utility.Core;

namespace NodeMessaging
{
    public class RootNode : IParentNode, IEndNode
    {
        private readonly List<NodeMessage> _nodeMessages = new List<NodeMessage>();
        private readonly Dictionary<Type, object> _registeredTypes = new Dictionary<Type, object>();
        private List<ParentNodeWrapper> _parentNodeWrappers;

        public IEnumerable<IParentNode> Nodes(string nameFilter)
        {
            if (_registeredTypes.ContainsKey(typeof(IParentNode)))
            {
                if (_parentNodeWrappers == null)
                {
                    var nodeHandler = (IParentNode) _registeredTypes[typeof (IParentNode)];
                    _parentNodeWrappers =
                        nodeHandler.Nodes(nameFilter).Select(node => new ParentNodeWrapper(this, node)).ToList();
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

        public void Register<T>(T t)
        {
            var interfaces = t.GetType().GetInterfaces().Select(i => AdditionalInterfaces(i, t)).Flatten();
            interfaces.Do(i => _registeredTypes[i] = t);
        }

        private void RegisterWithWrapper(Type typeToSupport, object obj)
        {
            var proxy = new ProxyGenerator();
            _registeredTypes[typeToSupport] = proxy.CreateInterfaceProxyWithoutTarget(typeToSupport, new SuperCast(obj));
        }

        private IEnumerable<Type> AdditionalInterfaces(Type type, object obj)
        {
            yield return type;
            if (type.IsGenericType)
            {
                if (type.GetGenericArguments().Count() == 1)
                {
                    var subtype = type.GetGenericArguments().First();
                    var interfaces = subtype.GetInterfaces();
                    if (interfaces.Count() > 0)
                    {
                        var genericType = type.GetGenericTypeDefinition();
                        var arg = genericType.GetGenericArguments().First();
                        var constraint = arg.GetGenericParameterConstraints().First();
                        var typeToLookFor = interfaces
                            .Where(i => constraint == i)
                            .Select(i => genericType.MakeGenericType(i))
                            .SingleOrDefault();

                        if (typeToLookFor != null)
                        {
                            var specialType = type.Assembly.GetTypes().SingleOrDefault(typeToLookFor.IsAssignableFrom);
                            RegisterWithWrapper(specialType, obj);
                        }

                    }
                }
            }
            yield break;
        }

        private IEnumerable<INode> DescendantNodes()
        {
//            Nodes();
            return null;
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
/*            var msgOut = Message.Create(lambda, invocation.Method.DeclaringType);
            msgOut.Invoke(message.Target);*/

            var del = message.TargetDelegate;
            var action = del.GetType().GetMethod("Invoke").Invoke(del, new object[] {invocation});
            action.GetType().GetMethod("Invoke").Invoke(action, new [] {message.Target});
        }
    }
}