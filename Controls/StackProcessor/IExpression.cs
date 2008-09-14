using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using VIMControls.Contracts;
using VIMControls.Controls.StackProcessor.MathExpressions;

namespace VIMControls.Controls.StackProcessor
{
    public interface IExpression
    {
    }

    public interface INumericExpression : ISystemExpression
    {
        double dVal { get; }
    }

    public interface IFuncExpression : IExpression
    {
        IExpression Eval(IEnumerable<IExpression> args);
        int StackArgs { get; }
    }

    public class TextPointerExpression : IExpression
    {}

    public abstract class MathExpression<TFunc> : IFuncExpression
    {
        protected readonly Expression<TFunc> _fn;
        protected IEnumerable<IExpression> _args;

        protected MathExpression(Expression<TFunc> fn)
        {
            _fn = fn;
        }

        public IExpression Eval(IEnumerable<IExpression> args)
        {
            var nargs = args.OfType<INumericExpression>();
            if (nargs.Count() < StackArgs)
            {
                _args = args;
                return this;
            }
            return new DoubleExpression(MathEval(nargs));
        }

        protected abstract double MathEval(IEnumerable<INumericExpression> args);

        public int StackArgs { get { return typeof (TFunc).GetGenericArguments().Count() - 1; } }
    }

    public class UnaryMathExpression : MathExpression<Func<double, double>>
    {
        public UnaryMathExpression(Expression<Func<double, double>> fn) : base(fn)
        {
        }

        protected override double MathEval(IEnumerable<INumericExpression> args)
        {
            return _fn.Compile()(args.Single().dVal);
        }
    }

    public class NullExpression : IFuncExpression
    {
        private readonly Action _fn;

        public NullExpression(Action fn)
        {
            _fn = fn;
        }

        public IExpression Eval(IEnumerable<IExpression> args)
        {
            _fn();
            return null;
        }

        public int StackArgs
        {
            get { return 0; }
        }
    }

    public interface ISystemExpression : IExpression
    {
        Delegate GetDelegate();
        Expression GetParameterizedExpression();
        IEnumerable<string> ParameterList { get; }
    }

    public class ExpressionIdentityAttribute : Attribute
    {
        public string Identity { get; set; }
        public ExpressionType SystemType { get; set; }

        public ExpressionIdentityAttribute(string identity, ExpressionType systemType)
        {
            Identity = identity;
            SystemType = systemType;
        }
    }

    public static class ExpressionMaps
    {
        [StackCmdRegister("mru")]
        public static void mru(IVIMStack stack)
        {
            VIMMessageService.SendMessage<IVIMControlContainer>(a => a.Navigate("mru"));
        }
    }

    public class StackCmdRegisterAttribute : Attribute
    {
        public string Name { get; private set; }
        public StackCmdRegisterAttribute(string name)
        {
            Name = name;
        }
    }

    public static class VIMExpression
    {
        private static readonly Dictionary<string, IExpression> _expressions = SetupExpressionDictionary();

        private static Dictionary<string, IExpression> SetupExpressionDictionary()
        {
            var dict = new Dictionary<string, IExpression>
                                                                                   {
                                                                                       {"pow", StackOpExpression.Power},
                                                                                       {"log", StackOpExpression.Log},
                                                                                       {"sin", StackOpExpression.Sin},
                                                                                       {"cos", StackOpExpression.Cos},
                                                                                       {"tan", StackOpExpression.Tan},
                                                                                       {"asin", StackOpExpression.ASin},
                                                                                       {"acos", StackOpExpression.ACos},
                                                                                       {"atan", StackOpExpression.ATan},
                                                                                       {"pi", new DoubleExpression(Math.PI)},
                                                                                       {"e", new DoubleExpression(Math.E)},
                                                                                       {"reset", new NullExpression(Reset)},
                                                                                       {"eval", StackOpExpression.EvalExp},
                                                                                       {"sto", new StoExpression()},
                                                                                       {"stoa", new StoExpression()},
                                                                                       {"cmd", new StoExpression()},
                                                                                       {"del", StackOpExpression.Delete},
                                                                                       {"swap", StackOpExpression.Swap},
                                                                                       {"graph", StackOpExpression.Graph},
                                                                                       {"edit", StackOpExpression.Edit},
                                                                                       {"array", StackOpExpression.Array},
                                                                                       {"regparser", StackOpExpression.RegParser},
                                                                                       {"fn", StackOpExpression.Fn}
                                                                                   };

            var methods = typeof (IExpression).Assembly.GetMethodsWithCustomAttribute<StackCmdRegisterAttribute>();
            methods.Select(method => new {name = method.AttributesOfType<StackCmdRegisterAttribute>().Single().Name,
                expr = new StackOpExpression(s => method.Invoke(null, new object[]{s}))})
                .Do(a => dict[a.name] = a.expr);

            return dict;
        }

        private static void Reset()
        {
            VIMMessageService.SendMessage<IVIMControlContainer>(c => c.Navigate("."));
        }

        public static IExpression FromString(string expr)
        {
            var val = StoExpression.GetValue(expr);
            return val ?? (_expressions.ContainsKey(expr) ? _expressions[expr] : new StringExpression(expr));
        }

        public static IExpression GetFunc<TExpr>() where TExpr : IFuncExpression
        {
            return Services.Locate<TExpr>()();
        }
    }

    public interface IVIMStack
    {
        IExpression Pop();
        void Push(IExpression expression);
        int Count { get; }
    }
}