using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
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
        public static StackOpExpression RegParser = new StackOpExpression(fnRegParser);
        public static StackOpExpression Fn = new StackOpExpression(fnFn);

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

        private static void fnRegParser(IVIMStack stack)
        {
            var parserName = stack.Pop().ToString();
            var parserText = stack.Pop().ToString();
            var dict = new Dictionary<string, string> {{parserName, parserText}};
            var guid = new Guid("{9AAC2521-41C8-4a26-8A74-DFBB7FA85ADE}");
            dict.Persist(guid);
        }

        private static void fnFn(IVIMStack stack)
        {
            var numExpr = (DoubleExpression) stack.Pop();
            var numArgs = (int) numExpr.dVal;

            var exprs = Enumerable.Range(0, numArgs)
                .Select(i => stack.Pop())
                .Cast<StringExpression>()
                .Reverse()
                .ToList();

            stack.Push(new FunctionExpression(exprs, (ISystemExpression)stack.Pop()));
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

    public class FunctionExpression : ISystemExpression
    {
        private readonly List<string> _argNames;
        private readonly ISystemExpression _evaluator;

        public FunctionExpression(IEnumerable<StringExpression> args, ISystemExpression evaluator)
        {
            _argNames = args.Select(expr => expr.ToString()).ToList();
            _evaluator = evaluator;
        }

        public override string ToString()
        {
            return "f(" + _argNames.Delimit(",") + ") = " + _evaluator;
        }

        public Delegate GetDelegate()
        {
            var expr = _evaluator.GetParameterizedExpression();
            var l = (LambdaExpression) expr;
            //I could search through l, and replace any ParameterExpression where it's t with 4
            //something like l.TreeReplace()

            var inParams = l.Parameters.Select(pExpr => pExpr.Name);
            var lParams = inParams
                .Where(s => _argNames.Contains(s))
                .Select(s => Expression.Parameter(typeof(double), s))
                .AsEnumerable();

            var fn = ((Func<double, double, double>) _evaluator.GetDelegate());

            var ex = ((Expression<Func<double, double>>) (x => (fn(x, 4))));

            var ex2 = ((Expression<Func<double, Func<double, double, double>, double>>) ((x, f) => f(x, 4)));
            

/*            var memberinfo = ((MemberExpression) ((InvocationExpression) ex.Body).Expression).Member;

            var body = MakeExpression(fn, memberinfo);
            var lambda = Expression.Lambda(body, lParams.ToArray());
            return lambda.Compile();*/

            return ex.Compile();
        }

        private Expression MakeExpression(Func<double, double, double> fn, MemberInfo memberinfo)
        {
            var fnCon = Expression.Constant(fn);
            var parm = Expression.Parameter(typeof (double), "x");
            var con = Expression.Constant(4.0);
            var invoke = Expression.Invoke(fnCon, parm, con);
            return invoke;
        }

        public Expression GetParameterizedExpression()
        {
            return _evaluator.GetParameterizedExpression();
        }

        public IEnumerable<string> ParameterList
        {
            get { return _evaluator.ParameterList; }
        }
    }
}