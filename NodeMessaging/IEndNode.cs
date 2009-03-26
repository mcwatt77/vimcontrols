using System;
using System.Collections.Generic;
using ActionDictionary;

namespace NodeMessaging
{
    public interface IEndNode : INode
    {
        Message Send(Message message);
        IEnumerable<Type> RegisteredTypes { get; }
        void InstallHook<T>(T tHook, object recipient);
    }
}
