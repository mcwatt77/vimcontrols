using System;

namespace Navigator.Containers
{
    public interface IContainer
    {
        void Register(Type key, Type typeToInstantiate, ContainerRegisterType registerType);
        void RegisterInstance(Type key, object instance);
        TResult Get<TResult>(params object[] objects);
    }
}