using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ActionDictionary;

namespace DataProcessors.Tests
{
    public class RootNode : IParentNode
    {
        private IParentNode _baseNode;
        private readonly Dictionary<Type, object> _typeRegistry = new Dictionary<Type, object>();

        public IEnumerable<IParentNode> Nodes(string nameFilter)
        {
            return _baseNode.Nodes(nameFilter);
        }

        public IParentNode NodeAt(int index)
        {
            return _baseNode.NodeAt(index);
        }

        public IParentNode Attribute(string name)
        {
            return _baseNode.Attribute(name);
        }

        public T Get<T>()
        {
            return (T)_typeRegistry[typeof (T)];
        }

        public void Register<T>(T t)
        {
            if (typeof(IParentNode).IsAssignableFrom(typeof(T)))
            {
                _baseNode = (IParentNode)t;
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

        public void InstallHook<T>(string path, T recipient, Expression<Action<T, INode>> fnGetData)
        {}
        public void InstallHook<T, U>(string path, T recipient, Expression<Func<T, U>> fnGetData)
        {}
    }
}