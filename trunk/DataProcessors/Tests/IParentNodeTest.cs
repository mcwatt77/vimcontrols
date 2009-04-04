using System;
using System.Collections.Generic;
using ActionDictionary;

namespace DataProcessors.Tests
{
    public interface IParentNodeTest : INodeTest
    {
        IEnumerable<IParentNodeTest> Nodes(string nameFilter);
        IParentNodeTest NodeAt(int index);
        IParentNodeTest Attribute(string name);
    }

    public interface INodeTest
    {
        T Get<T>();
        void Register<T>(T t);
        Message Send(Message message);
        IEnumerable<Type> RegisteredTypes { get; }
        void InstallHook<T>(T tHook, object recipient);
    }
}