using System;
using System.Linq;
using System.Linq.Expressions;

namespace Navigator
{
    public class Message
    {
        private readonly LambdaExpression _expression;

        public static Message Build<T>(Expression<Action<T>> fn)
        {
            return new Message(fn);
        }

        public Message(LambdaExpression expression)
        {
            _expression = expression;
        }

        public bool CanHandle(object @object)
        {
            return _expression.Parameters.Single().Type.IsAssignableFrom(@object.GetType());
        }

        public Delegate Delegate
        {
            get
            {
                return _expression.Compile();
            }
        }

        public object Invoke(object @object)
        {
//            return Delegate.DynamicInvoke(@object);
            var messagable = @object as IMessageable;
            return messagable != null ? messagable.Execute(this) : Delegate.DynamicInvoke(@object);
        }
    }
}