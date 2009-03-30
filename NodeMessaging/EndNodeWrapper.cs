using System;
using System.Collections.Generic;
using ActionDictionary;
using Castle.Core.Interceptor;
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
            get { return _node.Name; }
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

/*            var generator = new ProxyGenerator();
            var proxy = (T) generator.CreateInterfaceProxyWithTarget(typeof (T), new[] {typeof (IEndNode)}, ret, new FinalInterceptor(_rootNode, _node));
            return proxy;*/
            return InjectNode(ret, Intercept);
        }

        //TODO: Remove duplication
        private static T InjectNode<T>(T t, Action<IInvocation> fnIntercept) where T : class
        {
            var generator = new ProxyGenerator();
            var proxy = (T) generator.CreateInterfaceProxyWithTarget(typeof (T), new[] {typeof (IEndNode)}, t, new DelegateInterceptor(fnIntercept));
            return proxy;
        }

        private Message Intercept(Message message)
        {
            if (message.MethodType == typeof(INode))
            {
                message.Invoke(_node);
                return null;
            }
            return message;
        }

        //TODO: Remove duplication
        private void Intercept(IInvocation invocation)
        {
            //invocation
            //.TargetType, .Method, .ReturnValue, .Arguments, .Proceed()

            if (invocation.TargetType != typeof(INode) && invocation.Method.DeclaringType == typeof(INode))
            {
                invocation.ReturnValue = invocation.Method.Invoke(_node, invocation.Arguments);
                return;
            }
            invocation.Proceed();
            _rootNode.Intercept(_node, invocation);
        }
    }
}