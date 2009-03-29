using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Core.Interceptor;
using Castle.DynamicProxy;
using Utility.Core;

namespace NodeMessaging
{
    public class ParentNodeWrapper : IParentNode
    {
        private readonly RootNode _rootNode;
        private readonly IParentNode _node;
        private readonly Dictionary<Type, object> _registeredTypes = new Dictionary<Type, object>();

        public ParentNodeWrapper(RootNode rootNode, IParentNode node)
        {
            _rootNode = rootNode;
            _node = node;
        }

        public string Name
        {
            get { throw new System.NotImplementedException(); }
        }

        public IEnumerable<IParentNode> Nodes(string nameFilter)
        {
            throw new System.NotImplementedException();
        }

        public IParentNode NodeAt(int index)
        {
            throw new System.NotImplementedException();
        }

        public IEndNode Attribute(string name)
        {
            //bug:  This should not be newing up every time... Memoize that stuff!
            return new EndNodeWrapper(_rootNode, _node.Attribute(name));
        }

        public T Get<T>() where T : class
        {
            var ret = (T) _registeredTypes[typeof (T)];
            return InjectNode(ret, Intercept);
        }

        //TODO: Remove duplication
        private void Intercept(IInvocation invocation)
        {
            if (invocation.TargetType != typeof(INode) && invocation.Method.DeclaringType == typeof(INode))
            {
                invocation.ReturnValue = invocation.Method.Invoke(_node, invocation.Arguments);
                return;
            }
            invocation.Proceed();
            _rootNode.Intercept(_node, invocation);
        }

        //TODO: Remove duplication
        private static T InjectNode<T>(T t, Action<IInvocation> fnIntercept) where T : class
        {
            var generator = new ProxyGenerator();
            var interfaces = t.GetType().GetInterfaces().Concat(new[] {typeof (IEndNode)}).ToArray();
            var proxy = (T) generator.CreateInterfaceProxyWithTarget(typeof(T), interfaces, t, new DelegateInterceptor(fnIntercept));
            //TODO: Include all child interfaces in Proxy
            return proxy;
        }

        public void Register<T>(T t)
        {
            var interfaces = t.GetType().GetInterfaces();
            interfaces.Do(i => _registeredTypes[i] = t);
        }

        public IParentNode Parent
        {
            get { throw new System.NotImplementedException(); }
        }
    }
}