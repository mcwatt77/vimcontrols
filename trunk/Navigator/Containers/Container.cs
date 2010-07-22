using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Castle.Core.Interceptor;
using Castle.DynamicProxy;
using Utility.Core;
using VIControls.Commands;

namespace Navigator.Containers
{
    public class Container : IContainer
    {
        private readonly Dictionary<Type, Func<object[], object>> _dictionary = new Dictionary<Type, Func<object[], object>>();
        private readonly Dictionary<Type, object> _singletonDictionary = new Dictionary<Type, object>();
        private readonly Dictionary<string, Func<object[], object>> _nameDictionary = new Dictionary<string, Func<object[], object>>();
        private readonly Dictionary<Type, HashSet<Type>> _interceptTypeChain = new Dictionary<Type, HashSet<Type>>();

        public Container()
        {
            _dictionary[typeof (IContainer)] = objects => this;
        }

        private object ExecuteForType(Type type, object[] objects, IContainerIntercept intercept)
        {
            if (intercept != null)
                return typeof (IContainerIntercept)
                    .GetMethod("Get")
                    .MakeGenericMethod(type)
                    .Invoke(intercept, new object[] {objects});

            if (!_dictionary.ContainsKey(type)) throw new ComponentNotFound(type);

            return _dictionary[type](objects);
        }

        public void Register(Type key, Type typeToInstantiate, ContainerRegisterType registerType)
        {
            if (registerType == ContainerRegisterType.Singleton)
                _dictionary[key] = objects =>
                                       {
                                           if (!_singletonDictionary.ContainsKey(key))
                                               _singletonDictionary[key] = BuildObject(objects, typeToInstantiate);
                                           return _singletonDictionary[key];
                                       };

            if (registerType == ContainerRegisterType.Instance)
                _dictionary[key] = objects => { return BuildObject(objects, typeToInstantiate); };

            if (registerType == ContainerRegisterType.Intercept)
            {
                var list = _interceptTypeChain.ContainsKey(key)
                               ? _interceptTypeChain[key]
                               : (_interceptTypeChain[key] = new HashSet<Type>());

                if (!list.Contains(typeToInstantiate)) list.Add(typeToInstantiate);

                _dictionary[key] = objects => { return GetProxiedObject(key, objects, typeToInstantiate); };
            }
        }

        private object GetProxiedObject(Type key, IEnumerable<object> objects, Type typeToInstantiate)
        {
            var interceptObject = Get(typeToInstantiate) as IContainerIntercept;

            var parameters = GetParametersList(key, objects, interceptObject);

            var generator = new ProxyGenerator();
            var options = new ProxyGenerationOptions();
            var interceptor = new ProxyInterceptor(key);

            var descendants = key
                .Descendants(type => _interceptTypeChain.ContainsKey(type) ? _interceptTypeChain[type] : null)
                .ToArray();

            var proxiedObject = generator.CreateClassProxy(key, descendants, options, parameters, interceptor);
            interceptor.InterceptObject = interceptObject;

            var initialize = interceptor.InterceptObject as IInitialize;
            if (initialize != null) initialize.Initialize(proxiedObject);

            return proxiedObject;
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
            private readonly Type _oldType;

            public ProxyInterceptor(Type type)
            {
                _oldType = type;
            }

            public object InterceptObject { get; set; }

            public void Intercept(IInvocation invocation)
            {
                if (!invocation.Method.DeclaringType.IsAssignableFrom(_oldType))
                {
                    var lambda = ConvertToLambda(invocation);
                    var message = new Message(lambda);
                    invocation.ReturnValue = message.Invoke(InterceptObject);
                }
                else invocation.Proceed();
            }

            private static LambdaExpression ConvertToLambda(IInvocation invocation)
            {
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
                var parametersList = GetParametersList(typeToInstantiate, objects, null);
                return Activator.CreateInstance(typeToInstantiate, parametersList);
            }
            catch (Exception e)
            {
                throw new Exception("Could not instantiate type '" + typeToInstantiate + "' for: " + e.Message, e);
            }
        }

        private object[] GetParametersList(Type typeToInstantiate, IEnumerable<object> objects, IContainerIntercept containerIntercept)
        {
            var constructor = GetConstructor(typeToInstantiate);

            return GetListForConstructor(constructor, objects, containerIntercept);
        }

        private static ConstructorInfo GetConstructor(Type typeToInstantiate)
        {
            var parameterCount = typeToInstantiate
                .GetConstructors()
                .Max(constructorInfo => constructorInfo.GetParameters().Count());

            return typeToInstantiate
                .GetConstructors()
                .Single(constructorInfo => constructorInfo.GetParameters().Count() == parameterCount);
        }

        private object[] GetListForConstructor(ConstructorInfo constructor, IEnumerable<object> objects, IContainerIntercept intercept)
        {
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
                                                   Parameter = ExecuteForType(a.Parameter.ParameterType, new object[]{}, intercept)
                                               };
                                objectListCopy.Remove(objectToRemove);
                                return new {a.Index, Parameter = objectToRemove};
                            })
                .OrderBy(a => a.Index)
                .Select(a => a.Parameter)
                .ToArray();
        }

        private object Get(Type type, params object[] objects)
        {
            return typeof (Container)
                .GetMethods()
                .Single(method => method.Name == "GetOrDefault" && method.GetParameters().Count() == 1)
                .MakeGenericMethod(type)
                .Invoke(this, new object[] {objects});
        }

        public TResult Get<TResult>(params object[] objects)
        {
            try
            {
                return (TResult) ExecuteForType(typeof (TResult), objects, null);
            }
            catch (ComponentNotFound e)
            {
                throw new Exception("Failed instantiating '" + typeof (TResult) + "' for " + e.Message, e);
            }
            catch (Exception e)
            {
                throw new Exception("Failed instantiating '" + typeof(TResult).Name + "'", e);
            }
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