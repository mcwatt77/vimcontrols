using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ActionDictionary;

namespace DataProcessors.Tests
{
    public class RootNodeTest : IParentNodeTest
    {
        private IParentNodeTest _baseNode;
        private readonly Dictionary<Type, object> _typeRegistry = new Dictionary<Type, object>();

        public IEnumerable<IParentNodeTest> Nodes(string nameFilter)
        {
            return _baseNode.Nodes(nameFilter);
        }

        public IParentNodeTest NodeAt(int index)
        {
            return _baseNode.NodeAt(index);
        }

        public IParentNodeTest Attribute(string name)
        {
            return _baseNode.Attribute(name);
        }

        public T Get<T>()
        {
            return (T)_typeRegistry[typeof (T)];
        }

        public void Register<T>(T t)
        {
            if (typeof(IParentNodeTest).IsAssignableFrom(typeof(T)))
            {
                _baseNode = (IParentNodeTest)t;
            }
            else
            {
                _typeRegistry[typeof (T)] = t;
            }
        }

        public Message Send(Message message)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<Type> RegisteredTypes
        {
            get { return _typeRegistry.Keys; }
        }

        public void InstallHook<T>(T tHook, object recipient)
        {
            throw new System.NotImplementedException();
        }

        public void InstallHook<T>(string path, T recipient, Expression<Action<T, INodeTest>> fnGetData)
        {}
        public void InstallHook<T, U>(string path, T recipient, Expression<Func<T, U>> fnGetData)
        {}
    }
}