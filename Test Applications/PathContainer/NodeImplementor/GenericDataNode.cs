using System;
using PathContainer.Node;

namespace PathContainer.NodeImplementor
{
    public class GenericDataNode : IDataNode
    {
        public static IDataNode Create(Func<object> fnNew)
        {
            return new GenericDataNode{Data = fnNew()};
        }

        public object Data { get; private set; }

        public T Cast<T>()
        {
            return (T) Data;
        }
    }
}