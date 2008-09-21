using System.Linq;
using System.Linq.Expressions;
using VIMControls.Interfaces;

namespace VIMControls.Input
{
    public class Command : ICommand
    {
        private readonly LambdaExpression _expression;

        public Command(LambdaExpression expression)
        {
            _expression = expression;
        }

        public void Invoke(ICommandable commandable)
        {
            if (_expression.Parameters.Single().Type.IsAssignableFrom(commandable.GetType()))
            {
                var del = _expression.Compile();
                del.DynamicInvoke(commandable);
            }
            else
                commandable.ProcessMissingCommand(this);
        }
    }
}