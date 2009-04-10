using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Core.Interceptor;
using Castle.DynamicProxy;
using Utility.Core;

namespace NodeMessaging
{
    public class NodeBase
    {
        protected readonly Dictionary<Type, object> _registeredTypes = new Dictionary<Type, object>();
        protected readonly RootNode _rootNode;
        protected readonly INodeImplementor _node;

        public NodeBase(RootNode rootNode, INodeImplementor node)
        {
            _rootNode = rootNode;
            _node = node;
        }

        public virtual T Get<T>() where T : class
        {
            var ret = _registeredTypes.ContainsKey(typeof (T))
                          ? ((T) _registeredTypes[typeof (T)])
                          : _node.Get<T>();

            return InjectNode(ret, Intercept);
        }

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

        private static T InjectNode<T>(T t, Action<IInvocation> fnIntercept) where T : class
        {
            var generator = new ProxyGenerator();
            var interfaces = t.GetType().GetInterfaces().Concat(new[] {typeof (IEndNode)}).ToArray();
            var proxy = (T) generator.CreateInterfaceProxyWithTarget(typeof(T), interfaces, t, new DelegateInterceptor(fnIntercept));
            //TODO: Include all child interfaces in Proxy
            return proxy;
        }

        public virtual void Register<T>(T t)
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
                        var constraint = arg.GetGenericParameterConstraints().FirstOrDefault();
                        if (constraint != null)
                        {
                            var typeToLookFor = interfaces
                                .Where(i => constraint == i)
                                .Select(i => genericType.MakeGenericType(i))
                                .SingleOrDefault();

                            if (typeToLookFor != null)
                            {
                                var specialTypes = type
                                    .Assembly
                                    .GetTypes()
                                    .Where(typeToLookFor.IsAssignableFrom)
                                    .Where(ltype => ltype.IsInterface);
                                RegisterWithWrapper(specialTypes.SingleOrDefault(), obj);
                            }
                        }
                    }
                }
            }
            yield break;
        }
    }
}