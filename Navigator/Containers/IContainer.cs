using System;

namespace Navigator.Containers
{
    //TODO: Change this to work based on IContainerElements rather than new methods every time I want to do something new
    public interface IContainer : IReadContainer
    {
        void Register(Type key, Type typeToInstantiate, ContainerRegisterType registerType);
        void RegisterInstance(Type key, object instance);
        void RegisterByName(string name, Type typeToInstantiate, ContainerRegisterType registerType);
        TResult GetOrDefault<TResult>(params object[] objects);
        object GetOrDefault<TResult>(Func<Exception, object> fn, params object[] objects);
    }
}