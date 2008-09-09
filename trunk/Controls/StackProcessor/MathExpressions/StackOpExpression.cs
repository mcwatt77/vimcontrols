using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using VIMControls.Contracts;
using VIMControls.Controls.StackProcessor.Graphing;

namespace VIMControls.Controls.StackProcessor.MathExpressions
{
    public class ArrayExpression : ISystemExpression
    {
        private readonly List<IExpression> _expressions;

        public ArrayExpression(IEnumerable<IExpression> expressions)
        {
            _expressions = expressions.ToList();
        }

        public Delegate GetDelegate()
        {
            return null;
        }

        public Expression GetParameterizedExpression()
        {
            return null;
        }

        public IEnumerable<string> ParameterList
        {
            get { return new List<string>(); }
        }

        public override string ToString()
        {
            var text = "[";
            if (_expressions.Count > 0) text += _expressions.First();
            if (_expressions.Count > 1) text += ", " + _expressions.Skip(1).First();
            if (_expressions.Count > 2) text += ", ...";
            text += "]";
            return text;
        }

        public IEnumerable<IExpression> Items
        {
            get
            {
                return _expressions;
            }
        }
    }

    public class StackOpExpression : IFuncExpression
    {
        public static StackOpExpression Delete = new StackOpExpression(fnDelete);
        public static StackOpExpression Swap = new StackOpExpression(fnSwap);
        public static StackOpExpression EvalExp = new StackOpExpression(fnEval);
        public static StackOpExpression Add = new StackSystemExpression<AddExpression>();
        public static StackOpExpression Subtract = new StackSystemExpression<SubtractExpression>();
        public static StackOpExpression Multiply = new StackSystemExpression<MultiplyExpression>();
        public static StackOpExpression Divide = new StackSystemExpression<DivideExpression>();
        public static StackOpExpression Log = new StackSystemExpression<LogExpression>();
        public static StackOpExpression Power = new StackSystemExpression<PowerExpression>();
        public static StackOpExpression Graph = new StackOpExpression(fnGraph);
        public static StackOpExpression Sin = new StackSystemExpression<SinExpression>();
        public static StackOpExpression Cos = new StackSystemExpression<CosExpression>();
        public static StackOpExpression Tan = new StackSystemExpression<TanExpression>();
        public static StackOpExpression ASin = new StackSystemExpression<ASinExpression>();
        public static StackOpExpression ACos = new StackSystemExpression<ACosExpression>();
        public static StackOpExpression ATan = new StackSystemExpression<ATanExpression>();
        public static StackOpExpression Array = new StackOpExpression(fnArray);
        public static StackOpExpression Edit = new StackOpExpression(fnEdit);

        private readonly Action<IVIMStack> _fn;

        public StackOpExpression(Action<IVIMStack> fn)
        {
            _fn = fn;
        }

        public void Eval(IVIMStack stack)
        {
            _fn(stack);
        }

        private static void fnDelete(IVIMStack stack)
        {
            stack.Pop();
        }

        private static void fnSwap(IVIMStack stack)
        {
            var expr0 = stack.Pop();
            var expr1 = stack.Pop();
            stack.Push(expr0);
            stack.Push(expr1);
        }

        private static void fnEval(IVIMStack stack)
        {
            var sysExpr = (ISystemExpression)stack.Pop();
            var fn = sysExpr.GetDelegate();
            var parameters = Enumerable.Range(0, fn.Method.GetParameters().Count())
                .Select(i => stack.Pop())
                .Select(expression => ((DoubleExpression)expression).dVal)
                .Cast<object>()
                .ToArray();
            stack.Push(new DoubleExpression((double)fn.DynamicInvoke(parameters)));
        }

        private static void fnGraph(IVIMStack stack)
        {
            var sysExpr = (ISystemExpression)stack.Pop();
            if (sysExpr is ArrayExpression)
            {
                var arr = (ArrayExpression) sysExpr;
                var dItems = arr.Items
                    .OfType<DoubleExpression>()
                    .Select(de => de.dVal);

                var list = new GraphableList(dItems);
                VIMMessageService.SendMessage((Action<IVIMGraphPanel>)(a => a.Graph(list)));
                return;
            }

            var fn = sysExpr.GetDelegate();
            VIMMessageService.SendMessage((Action<IVIMGraphPanel>)(a => a.Graph(fn)));
        }

        private static void fnEdit(IVIMStack stack)
        {
            var text = String.Empty;
            if (stack.Count > 0) text = stack.Pop().ToString();
            VIMMessageService.SendMessage((Action<ITextInputProvider>)(a => a.Text = text));
        }

        private static void fnArray(IVIMStack stack)
        {
            var numExpr = (DoubleExpression)stack.Pop();
            var arrayVals = Enumerable.Range(0, (int) numExpr.dVal)
                .Select(i => stack.Pop());
            stack.Push(new ArrayExpression(arrayVals));
        }

        public IExpression Eval(IEnumerable<IExpression> args)
        {
            return null;
        }

        public int StackArgs
        {
            get { return 0; }
        }
    }
}