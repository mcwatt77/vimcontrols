using System;
using System.Linq.Expressions;
using System.Reflection;

namespace ActionDictionary
{
    public class Message
    {
        private object _fn;
        private Expression _expr;
        private Type _type;
        private MethodInfo _method;

        public static Message Create(LambdaExpression fn, Type type)
        {
            var compiled = fn.Compile();
            var msg = new Message
                          {
                              _fn = compiled,
                              _type = type,
                              _method = compiled.GetType().GetMethod("Invoke"),
                              _expr = fn
                          };
            return msg;
        }

        public static Message Create<TInterface>(Expression<Action<TInterface>> fn)
        {
            var compiled = fn.Compile();
            var msg = new Message
                          {
                              _fn = compiled,
                              _type = typeof (TInterface),
                              _method = compiled.GetType().GetMethod("Invoke"),
                              _expr = fn
                          };
            return msg;
        }

        public void Invoke<TInterface>(TInterface obj)
        {
            if (_type.IsAssignableFrom(obj.GetType()))
                _method.Invoke(_fn, new object[] {obj});
        }

        public override string ToString()
        {
            return _expr.ToString();
        }
    }
}