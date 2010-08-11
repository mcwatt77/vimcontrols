using System;
using System.Linq;
using System.Linq.Expressions;
using VIControls.Commands.Interfaces;

namespace VIControls.Commands
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
            var messagable = @object as IMessageable;
            if (messagable == null || /*!messagable.*/CanHandle(@object)) return Delegate.DynamicInvoke(@object);
            return messagable.Execute(this);
        }
    }
}