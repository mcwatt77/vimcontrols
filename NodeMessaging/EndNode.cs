using System;
using System.Collections.Generic;
using ActionDictionary;

namespace NodeMessaging
{
    public class EndNode : IEndNodeImplementor
    {
        private readonly Dictionary<Type, object> _registeredTypes = new Dictionary<Type, object>();

        public void Register<T>(T t)
        {
            _registeredTypes[typeof (T)] = t;
        }

        public Message Send(Message message)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<Type> RegisteredTypes
        {
            get { throw new System.NotImplementedException(); }
        }

        public void InstallHook<T>(T tHook, object recipient)
        {
            throw new System.NotImplementedException();
        }

        public T Get<T>() where T : class
        {
            return (T) _registeredTypes[typeof (T)];
        }

        public string Name { get; set; }

        public IParentNodeImplementor Parent { get; set; }
    }
}