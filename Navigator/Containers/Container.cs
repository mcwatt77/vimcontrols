using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Core.Interceptor;
using Castle.DynamicProxy;

namespace Navigator.Containers
{
    public class Container : IContainer
    {
        private readonly Dictionary<Type, Func<object[], object>> _dictionary = new Dictionary<Type, Func<object[], object>>();
        private readonly Dictionary<Type, object> _singletonDictionary = new Dictionary<Type, object>();
        private readonly Dictionary<string, Func<object[], object>> _nameDictionary = new Dictionary<string, Func<object[], object>>();

        public Container()
        {
            _dictionary[typeof (IContainer)] = objects => this;
        }

        public void Register(Type key, Type typeToInstantiate, ContainerRegisterType registerType)
        {
            if (registerType == ContainerRegisterType.Singleton)
                _dictionary[key] = objects =>
                                       {
                                           if (objects.Length > 0)
                                               throw new InvalidOperationException( "Singleton objects can't take parameters");
                                           if (!_singletonDictionary.ContainsKey(key))
                                               _singletonDictionary[key] = BuildObject(objects, typeToInstantiate);
                                           return _singletonDictionary[key];
                                       };

            if (registerType == ContainerRegisterType.Instance)
                _dictionary[key] = objects => { return BuildObject(objects, typeToInstantiate); };
            if (registerType == ContainerRegisterType.Intercept)
            {
                var oldValue = _dictionary[key];
                _dictionary[key] = objects =>
                                       {
                                           var args = objects;
                                           var constructor = key.GetConstructors().Single();
                                           if (constructor.GetParameters().Length == 1
                                               && constructor.GetParameters().Single().ParameterType == typeof(object[]))
                                               args = new object[] {objects};

                                           var generator = new ProxyGenerator();
                                           var options = new ProxyGenerationOptions();
                                           var interceptor = new ProxyInterceptor(oldValue);
                                           var proxiedObject = generator.CreateClassProxy(key, new[] {typeToInstantiate}, options, args, interceptor);
                                           interceptor.InterceptObject = BuildObject(new [] {proxiedObject}, typeToInstantiate);
                                           return proxiedObject;
                                       };
            }
        }

        public void RegisterInstance(Type key, object instance)
        {
            _dictionary[key] = objects => instance;
        }

        public void RegisterByName(string name, Type typeToInstantiate, ContainerRegisterType registerType)
        {
            if (registerType != ContainerRegisterType.Instance) throw new InvalidOperationException("Currently only support instance type name registrations");

            _nameDictionary[name] = objects => BuildObject(objects, typeToInstantiate);
        }

        public TReturn GetByName<TReturn>(string name, params object[] objects)
        {
            return (TReturn) _nameDictionary[name](objects);
        }

        private class ProxyInterceptor : IInterceptor
        {
            private readonly object _oldValue;

            public ProxyInterceptor(object oldValue)
            {
                _oldValue = oldValue;
            }

            public object InterceptObject { get; set; }

            public void Intercept(IInvocation invocation)
            {
                if (!invocation.Method.DeclaringType.IsAssignableFrom(_oldValue.GetType()))
                {
                    var result = invocation.Method.Invoke(InterceptObject, invocation.Arguments);
                    invocation.ReturnValue = result;
                }
                else invocation.Proceed();
            }
        }

        private object BuildObject(IEnumerable<object> objects, Type typeToInstantiate)
        {
            try
            {
                var constructor = typeToInstantiate.GetConstructors().Single();
                var parameters = constructor.GetParameters();
                var orderedParameters = Enumerable.Range(0, parameters.Length)
                    .Select(index => new {Index = index, Parameter = parameters[index]})
                    .OrderByDescending(
                    a => parameters.Where(
                             parameter => parameter.ParameterType.IsAssignableFrom(a.Parameter.ParameterType)).Count())
                    .ToArray();

                var objectListCopy = objects.ToList();

                var parametersList = orderedParameters
                    .Select(a =>
                                {
                                    var objectToRemove = objectListCopy.SingleOrDefault(o => a.Parameter.ParameterType.IsAssignableFrom(o.GetType()));
                                    if (objectToRemove == null)
                                        return new {
                                                       a.Index,
                                                       Parameter = _dictionary[a.Parameter.ParameterType](new object[] {})
                                                   };
                                    objectListCopy.Remove(objectToRemove);
                                    return new {a.Index, Parameter = objectToRemove};
                                })
                    .OrderBy(a => a.Index)
                    .Select(a => a.Parameter)
                    .ToArray();
                return Activator.CreateInstance(typeToInstantiate, parametersList);
            }
            catch (Exception e)
            {
                throw new Exception("Could not instantiate type '" + typeToInstantiate + "' for: " + e.Message, e);
            }
        }

        public TResult Get<TResult>(params object[] objects)
        {
            return (TResult)_dictionary[typeof(TResult)](objects);
        }

        public TResult GetOrDefault<TResult>(params object[] objects)
        {
            try
            {
                return (TResult)_dictionary[typeof(TResult)](objects);
            }
            catch
            {
                return default(TResult);
            }
        }
    }
}