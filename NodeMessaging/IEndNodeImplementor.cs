using System;
using System.Collections.Generic;
using ActionDictionary;

namespace NodeMessaging
{
    public interface IEndNodeImplementor : INodeImplementor
    {
        Message Send(Message message);
        IEnumerable<Type> RegisteredTypes { get; }
        void InstallHook<T>(T tHook, object recipient);
        object Value { get; }
        T Get<T>() where T : class;
        void Register<T>(T t);
    }
}