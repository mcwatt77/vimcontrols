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

        public IEnumerable<Message> Invoke<TInterface>(TInterface obj)
        {
            return Invoke(obj, true);
        }

        public IEnumerable<Message> Invoke<TInterface>(TInterface obj, bool throwOnException)
        {
            var errorMsgs = new List<Message>();
            try
            {
                if (MethodType.IsAssignableFrom(obj.GetType()))
                {
                    try
                    {
                        _method.Invoke(_fn, new object[] {obj});
                    }
                    catch (TargetInvocationException e)
                    {
                        throw e.InnerException;
                    }
                }
                else if (typeof (IMissing).IsAssignableFrom(obj.GetType()))
                    ((IMissing) obj).ProcessMissingCmd(this);
            }
            catch(Exception ex)
            {
                if (throwOnException) throw ex;
                errorMsgs.Add(Create<IError>(e => e.Report(ex.Message)));
            }

            return errorMsgs;
        }

        public override string ToString()
        {
            return _expr.ToString();
        }
    }
}