using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using ActionDictionary.Interfaces;

namespace ActionDictionary
{
    public class Message
    {
        private object _fn;
        private Expression _expr;
        public Type MethodType { get; private set; }
        private MethodInfo _method;
        private readonly List<Message> _errors = new List<Message>();

        public IEnumerable<Message> Errors { get { return _errors; } }

        public static Message Create(LambdaExpression fn, Type type)
        {
            var compiled = fn.Compile();
            var msg = new Message
                          {
                              _fn = compiled,
                              MethodType = type,
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
                              MethodType = typeof (TInterface),
                              _method = compiled.GetType().GetMethod("Invoke"),
                              _expr = fn
                          };
            return msg;
        }

        public object Invoke<TInterface>(TInterface obj)
        {
            return Invoke(obj, true);
        }

        public object Invoke<TInterface>(TInterface obj, bool throwOnException)
        {
            try
            {
                if (MethodType.IsAssignableFrom(obj.GetType()))
                    return _method.Invoke(_fn, new object[] {obj});
                if (typeof (IMissing).IsAssignableFrom(obj.GetType()))
                    return ((IMissing) obj).ProcessMissingCmd(this);
            }
            catch(Exception ex)
            {
                if (throwOnException) throw;
                _errors.Add(Create<IError>(e => e.Report(ex)));
            }
            return null;
        }

        public override string ToString()
        {
            return _expr.ToString();
        }
    }
}