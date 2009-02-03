using System;
using System.Linq;

namespace VIMControls.Controls.StackProcessor.MathExpressions
{
    public class StackSystemExpression<TSystem> : StackOpExpression where TSystem : ISystemExpression
    {
        public StackSystemExpression() : base(EvaluateSystemExpression)
        {
        }

        public static void EvaluateSystemExpression(IVIMStack stack)
        {
            var cons = typeof (TSystem).GetConstructors();

            var con = cons
                .Where(constructor => constructor.GetParameters().Count() <= stack.Count)
                .OrderByDescending(constructor => constructor.GetParameters().Count())
                .First();

            var parameters = con
                .GetParameters()
                .Select(p => stack.Pop())
                .Select(
                expression =>
                expression.GetType() == typeof (StringExpression)
                    ? new VariableExpression(expression.ToString())
                    : expression)
                .Reverse()
                .ToArray();

            var sysExpression = (ISystemExpression)con.Invoke(parameters);
            var fn = sysExpression.GetDelegate();
            if (fn.Method.GetParameters().Count() == 0)
                stack.Push(new DoubleExpression(((Func<double>) fn)()));
            else
                stack.Push(sysExpression);
        }
    }
}