using System;

namespace Navigator.Containers
{
    public interface IReadContainer
    {
        TResult Get<TResult>(params object[] objects);
        object Get(Type type, params object[] objects);
    }
}