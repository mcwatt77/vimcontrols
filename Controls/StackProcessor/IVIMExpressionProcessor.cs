using System;
using System.Collections.Generic;
using System.Linq;
using VIMControls.Contracts;
using VIMControls.Controls.Misc;

namespace VIMControls.Controls.StackProcessor
{
    public interface IVIMExpressionProcessor : IVIMControl, IVIMMultiLineTextDisplay, IVIMController
    {
        void Process(IExpression expr);
        void Eval(IFuncExpression expr);
        IExpression Pop();
    }

    public class ExpressionProcessor : VIMTextControl, IVIMExpressionProcessor
    {
        private readonly List<IExpression> _expressions = new List<IExpression>();

        public ExpressionProcessor()
        {
            ApplyBorders = true;
        }

        public void Process(IExpression expr)
        {
            if (expr is EvalExpression)
            {
                Eval((IFuncExpression)expr);
                return;
            }

            if (expr != null) _expressions.Add(expr);

            _textData
                .Skip(_expressions.Count)
                .Do(tb => tb.Text = String.Empty);

            Enumerable.Range(0, Math.Min(_expressions.Count, _textData.Length - 1))
                .Do(i => _textData[_expressions.Count - i - 1].Text = _expressions[i].ToString());
        }

        public void Eval(IFuncExpression expr)
        {
            if (expr is IEvalExpression)
            {
                var eval = (IEvalExpression) expr;
                eval.SetStackArgs(_expressions.Last());
                _expressions.RemoveAt(_expressions.Count - 1);
            }

            var vals = _expressions
                .AsEnumerable()
                .Reverse()
                .Take(expr.StackArgs)
                .ToList();

            var eVal = expr.Eval(vals);
            _expressions.RemoveRange(_expressions.Count - expr.StackArgs, expr.StackArgs);
            if (expr is NullExpression) return;
            Process(eVal);
        }

        public IExpression Pop()
        {
            var expr = _expressions.Last();
            _expressions.RemoveAt(_expressions.Count - 1);
            return expr;
        }
    }

    public class FancyDisplayStack : VIMTextControl, IFancyDisplayStack
    {
        public void Push(IExpression expr)
        {
            Text = expr.ToString();
        }
    }

    public interface IFancyDisplayStack
    {
        void Push(IExpression expr);
    }
}