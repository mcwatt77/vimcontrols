using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
                                           var parameters = GetParametersList(key, objects);

                                           var generator = new ProxyGenerator();
                                           var options = new ProxyGenerationOptions();
                                           var interceptor = new ProxyInterceptor(oldValue);
                                           var proxiedObject = generator.CreateClassProxy(key, new[] {typeToInstantiate}, options, parameters, interceptor);
                                           interceptor.InterceptObject = BuildObject(new [] {proxiedObject}, typeToInstantiate);
                                           var initializable = interceptor.InterceptObject as IInitialize;
                                           if (initializable != null) initializable.Initialize();
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
                    var lambda = ConvertToLambda(invocation);
                    var message = new Message(lambda);
                    invocation.ReturnValue = message.Invoke(InterceptObject);
                }
                else invocation.Proceed();
            }

            private static LambdaExpression ConvertToLambda(IInvocation invocation)
            {
                Expression<Func<int, int>> fn = x => 1 + x;
                var parameter = Expression.Parameter(invocation.Method.DeclaringType, "x");
                var invocationExpression = Expression.Call(parameter, invocation.Method, invocation.Arguments.Select(o => Expression.Constant(o)).ToArray());
                var lambda = Expression.Lambda(invocationExpression, parameter);
                return lambda;
            }
        }

        private object BuildObject(IEnumerable<object> objects, Type typeToInstantiate)
        {
            try
            {
                var parametersList = GetParametersList(typeToInstantiate, objects);
                return Activator.CreateInstance(typeToInstantiate, parametersList);
            }
            catch (Exception e)
            {
                throw new Exception("Could not instantiate type '" + typeToInstantiate + "' for: " + e.Message, e);
            }
        }

        private object[] GetParametersList(Type typeToInstantiate, IEnumerable<object> objects)
        {
            var constructor = typeToInstantiate.GetConstructors().Single();
            var parameters = constructor.GetParameters();
            if (parameters.Length == 1 && parameters.Single().ParameterType == typeof(object[]))
            {
                return new object[] {objects};
            }

            var orderedParameters = Enumerable.Range(0, parameters.Length)
                .Select(index => new {Index = index, Parameter = parameters[index]})
                .OrderByDescending(
                a => parameters.Where(
                         parameter => parameter.ParameterType.IsAssignableFrom(a.Parameter.ParameterType)).Count())
                .ToArray();

            var objectListCopy = objects.ToList();

            return orderedParameters
                .Select(a =>
                            {
                                var objectToRemove = objectListCopy
                                    .Where(o => o != null)
                                    .SingleOrDefault(o => a.Parameter.ParameterType.IsAssignableFrom(o.GetType()));
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
        }

        public TResult Get<TResult>(params object[] objects)
        {
            return (TResult)_dictionary[typeof(TResult)](objects);
        }

        public TResult GetOrDefault<TResult>(params object[] objects)
        {
            try
            {
                if (!_dictionary.ContainsKey(typeof(TResult)))
                {
                    Register(typeof (TResult), typeof (TResult), ContainerRegisterType.Instance);
                }
                return (TResult)_dictionary[typeof(TResult)](objects);
            }
            catch (Exception e)
            {
                return default(TResult);
            }
        }

        public object GetOrDefault<TResult>(Func<Exception, object> fn, params object[] objects)
        {
            try
            {
                return (TResult)_dictionary[typeof(TResult)](objects);
            }
            catch(Exception e)
            {
                return fn(e);
            }
        }
    }
}