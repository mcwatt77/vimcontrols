using System;
using System.Linq.Expressions;
using System.Reflection;
using ActionDictionary.Interfaces;

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
            {
                try
                {
                    _method.Invoke(_fn, new object[] {obj});
                }
                catch(TargetInvocationException e)
                {
                    throw e.InnerException;
                }
            }
            else if (typeof(IMissing).IsAssignableFrom(obj.GetType()))
                ((IMissing)obj).ProcessMissingCmd(this);
        }

        public override string ToString()
        {
            return _expr.ToString();
        }
    }
}