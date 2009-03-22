using System;
using System.Collections.Generic;
using ActionDictionary;

namespace NodeMessaging
{
    public interface IEndNode : INode
    {
        void Register<T>(T t);
        Message Send(Message message);
        IEnumerable<Type> RegisteredTypes { get; }
        void InstallHook<T>(T tHook, object recipient);
        T Get<T>() where T : class;
    }
}
