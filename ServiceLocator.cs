using System;
using System.Collections.Generic;
using System.Linq;

using VIMControls.Controls;

namespace VIMControls
{
    public class ServiceLocator
    {
        //is this going to be a threading issue?
        private static Dictionary<Type, object> _serviceRegistry = new Dictionary<Type, object>();

/*       If I uncommented, 
 * public static Func<T> FindService<T>(string serviceName)
        {
            return () => default(T);
        }*/

        public static Func<T> FindService<T>(params object[] dependencies)
        {
            if (_serviceRegistry.ContainsKey(typeof(T)))
            {
                return () => (T)_serviceRegistry[typeof (T)];
            }
                    
            var factories = typeof (IFactory<T>).GetImplementations();
            if (factories.Count() == 1)
            {
                var obj = factories.Single().GetConstructor(Type.EmptyTypes).Invoke(new object[] {});
                var methodInfo = factories.Single().GetMethods().Where(method => method.Name == "Create").Single();
                return () => (T)methodInfo.Invoke(obj, dependencies);
            }

            //create a factory of interfaceType
            //IFactory<T> factory = 
            var types = typeof (ServiceLocator).Assembly
                .GetTypes()
                .Where(type => typeof (T).IsAssignableFrom(type) &&
                               !type.IsAbstract);

            if (types.Count() == 1)
            {
                var constructor = types.Single().GetConstructor(dependencies.Select(o => o.GetType()).ToArray());
                if (constructor == null) return null;
                return () => (T)constructor.Invoke(dependencies);
            }
            return null;
        }

        public static void Register<T>(object serviceResponder)
        {
            if (_serviceRegistry.ContainsKey(typeof(T))) throw new Exception("Ugh.  You've blown my mind!");

            _serviceRegistry[typeof (T)] = serviceResponder;
        }
    }

    public class ServiceResult
    {}
}
