using System;
using System.Reflection;

namespace ActionDictionary
{
    public class Message
    {
        private object _fn;
        private Type _type;
        private MethodInfo _method;

        public static Message Create<TInterface>(Action<TInterface> fn)
        {
            var msg = new Message
                          {
                              _fn = fn,
                              _type = typeof (TInterface),
                              _method = fn.GetType().GetMethod("Invoke")
                          };
            return msg;
        }

        public void Invoke<TInterface>(TInterface obj)
        {
            if (_type.IsAssignableFrom(obj.GetType()))
                _method.Invoke(_fn, new object[] {obj});
        }
    }
}