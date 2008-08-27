using System;
using System.Linq;

using VIMControls.Controls;

namespace VIMControls
{
    public class ServiceLocator
    {
        public static Func<T> FindService<T>(string serviceName)
        {
            return () => default(T);
        }

        public static Func<T> FindService<T>(params object[] dependencies)
        {
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
                .Where(type => typeof(T).IsAssignableFrom(type));

            if (types.Count() == 1)
            {
                var constructor = types.Single().GetConstructor(Type.EmptyTypes);
                return () => (T)constructor.Invoke(new object[] {});
            }
            return null;
        }
    }

    public class ServiceResult
    {}
}
