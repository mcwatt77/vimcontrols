using System;
using System.Collections.Generic;
using System.Linq;
using Castle.DynamicProxy;
using Utility.Core;

namespace NodeMessaging
{
    public class NodeBase
    {
        protected readonly Dictionary<Type, object> _registeredTypes = new Dictionary<Type, object>();

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