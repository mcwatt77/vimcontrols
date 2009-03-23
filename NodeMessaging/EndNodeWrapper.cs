using System;
using System.Collections.Generic;
using ActionDictionary;
using Castle.DynamicProxy;

namespace NodeMessaging
{
    public class EndNodeWrapper : IEndNode
    {
        private readonly RootNode _rootNode;
        private readonly IEndNode _node;

        public EndNodeWrapper(RootNode rootNode, IEndNode node)
        {
            _rootNode = rootNode;
            _node = node;
        }

        public string Name
        {
            get { throw new System.NotImplementedException(); }
        }

        public IParentNode Parent
        {
            get { throw new System.NotImplementedException(); }
        }

        public void Register<T>(T t)
        {
            throw new System.NotImplementedException();
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

        public T Get<T>() where T : class
        {
            var ret = _node.Get<T>();

            var generator = new ProxyGenerator();
            var proxy = (T) generator.CreateInterfaceProxyWithTarget(typeof (T), new[] {typeof (IEndNode)}, ret, new FinalInterceptor(_rootNode, _node));
            return proxy;
        }
    }
}