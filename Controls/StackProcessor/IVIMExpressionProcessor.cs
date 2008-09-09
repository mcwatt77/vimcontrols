using System;
using System.Collections.Generic;
using System.Linq;
using VIMControls.Contracts;
using VIMControls.Controls.Misc;
using VIMControls.Controls.StackProcessor.MathExpressions;

namespace VIMControls.Controls.StackProcessor
{
    public interface IVIMExpressionProcessor : IVIMControl, IVIMMultiLineTextDisplay, IVIMController
    {
        void Push(IExpression expr);
        void Eval(IFuncExpression expr);
        IExpression Pop();
    }

    public class ExpressionProcessor : VIMTextControl, IVIMExpressionProcessor, IVIMStack
    {
        private readonly List<IExpression> _expressions = new List<IExpression>();

        public ExpressionProcessor()
        {
            ApplyBorders = true;
        }

        public void Push(IExpression expr)
        {
            if (expr != null) _expressions.Add(expr);

            RefreshStackView();
        }

        public int Count
        {
            get { return _expressions.Count; }
        }

        private void RefreshStackView()
        {
            _textData
                .Skip(_expressions.Count)
                .Do(tb => tb.Text = String.Empty);

            Enumerable.Range(0, Math.Min(_expressions.Count, _textData.Length - 1))
                .Do(i => _textData[i].Text = _expressions[_expressions.Count - i - 1].ToString());
        }

        public void Eval(IFuncExpression expr)
        {
            if (expr is StackOpExpression)
            {
                var st = (StackOpExpression) expr;
                st.Eval(this);

                //maybe this should be some sort of message?
                RefreshStackView();
                return;
            }

            var vals = _expressions
                .AsEnumerable()
                .Reverse()
                .Take(expr.StackArgs)
                .ToList();

            var eVal = expr.Eval(vals);
            _expressions.RemoveRange(_expressions.Count - expr.StackArgs, expr.StackArgs);
            if (expr is NullExpression) return;
            Push(eVal);
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