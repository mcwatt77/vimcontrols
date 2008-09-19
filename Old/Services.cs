using System;
using System.Collections.Generic;
using System.Linq;

namespace VIMControls
{
    public class Services
    {
        //is this going to be a threading issue?
        private static readonly Dictionary<Type, object> _serviceRegistry = new Dictionary<Type, object>();

        public static Func<T> Locate<T>(params object[] dependencies)
        {
            if (_serviceRegistry.ContainsKey(typeof(T)))
            {
                return () => (T)_serviceRegistry[typeof (T)];
            }

            var types = typeof (Services).Assembly
                .GetTypes()
                .Where(type => typeof (T).IsAssignableFrom(type) &&
                               !type.IsAbstract);
            if (types.Count() > 1)
                types = types.Where(type => type.BaseType == typeof(object));

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
}