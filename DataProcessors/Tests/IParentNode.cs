using System;
using System.Collections.Generic;
using ActionDictionary;

namespace DataProcessors.Tests
{
    public interface IParentNode : INode
    {
        IEnumerable<IParentNode> Nodes(string nameFilter);
        IParentNode NodeAt(int index);
        IParentNode Attribute(string name);
    }

    public interface INode
    {
        T Get<T>();
        void Register<T>(T t);
        Message Send(Message message);
        IEnumerable<Type> RegisteredTypes { get; }
        void InstallHook<T>(T tHook, object recipient);
    }
}