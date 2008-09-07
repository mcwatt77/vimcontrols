using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using VIMControls.Contracts;

namespace VIMControls.Controls.StackProcessor
{
    public interface IExpression
    {
    }

    public class DoubleExpression : INumericExpression
    {
        private readonly double _dVal;

        private DoubleExpression()
        {}

        public DoubleExpression(double dVal)
        {
            _dVal = dVal;
        }

        public double dVal
        {
            get { return _dVal; }
        }

        public override string ToString()
        {
            return _dVal.ToString();
        }

        public Delegate GetDelegate()
        {
            return (Func<double>)(() => _dVal);
        }

        public Expression GetParameterizedExpression()
        {
            return Expression.Constant(_dVal);
        }

        public int ParameterCount
        {
            get { return 0; }
        }
    }

    public class VariableExpression : ISystemExpression
    {
        private readonly string _name;

        public VariableExpression(string name)
        {
            _name = name;
        }

        public Delegate GetDelegate()
        {
            return (Func<double, double>) (x => x);
        }

        public Expression GetParameterizedExpression()
        {
            return Expression.Parameter(typeof (double), _name);
        }

        public int ParameterCount
        {
            get { return 1; }
        }

        public override string ToString()
        {
            return _name;
        }
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

    public class StoExpression : IFuncExpression
    {
        private const string _rpnStoGuid = "{56A84837-1459-48a5-96A5-26A9F532657F}";
        private Dictionary<string, Guid> _nameLookups = new Dictionary<string, Guid>();

        public static IExpression GetValue(string name)
        {
            var rpnGuid = new Guid(_rpnStoGuid);
            var nameLookups = rpnGuid.Load<Dictionary<string, Guid>>();
            if (nameLookups == null) return null;
            return !nameLookups.ContainsKey(name) ? null : nameLookups[name].Load<IExpression>();
        }

        public IExpression Eval(IEnumerable<IExpression> args)
        {
            var name = args.First().ToString();

            var guid = Guid.NewGuid();
            var rpnGuid = new Guid(_rpnStoGuid);
            _nameLookups = rpnGuid.Load<Dictionary<string, Guid>>() ?? new Dictionary<string, Guid>();
            if (!_nameLookups.ContainsKey(name))
            {
                _nameLookups[args.First().ToString()] = guid;
                _nameLookups.Persist(rpnGuid);
            }
            else
                guid = _nameLookups[name];

            args.Last().Persist(guid);
            return null;
        }

        public int StackArgs
        {
            get { return 2; }
        }
    }

    public class StringExpression : IExpression
    {
        private readonly string _expr;

        private StringExpression()
        {}

        public StringExpression(string expr)
        {
            _expr = expr;
        }

        public override string ToString()
        {
            return _expr;
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

    public class AddExpression : BinarySysExpression
    {
        public AddExpression(ISystemExpression op1, ISystemExpression op2) : base(op1, op2)
        {}

        protected override Func<double, double, double> F
        {
            get { return (d0, d1) => d0 + d1; }
        }

        protected override string Format
        {
            get { return "{0} + {1}"; }
        }
    }

    public class SubtractExpression : BinarySysExpression
    {
        public SubtractExpression(ISystemExpression op1, ISystemExpression op2) : base(op1, op2)
        {}

        protected override Func<double, double, double> F
        {
            get { return (d0, d1) => d0 - d1; }
        }

        protected override string Format
        {
            get { return "{0} - {1}"; }
        }
    }

    public class MultiplyExpression : BinarySysExpression
    {
        public MultiplyExpression(ISystemExpression op1, ISystemExpression op2) : base(op1, op2)
        {}

        protected override Func<double, double, double> F
        {
            get { return (d0, d1) => d0 * d1; }
        }

        protected override string Format
        {
            get { return "{0} * {1}"; }
        }
    }

    public class DivideExpression : BinarySysExpression
    {
        public DivideExpression(ISystemExpression op1, ISystemExpression op2) : base(op1, op2)
        {}

        protected override Func<double, double, double> F
        {
            get { return (d0, d1) => d0 / d1; }
        }

        protected override string Format
        {
            get { return "{0} / {1}"; }
        }
    }

    public class PowerExpression : BinarySysExpression
    {
        public PowerExpression(ISystemExpression op1, ISystemExpression op2) : base(op1, op2)
        {}

        protected override Func<double, double, double> F
        {
            get { return Math.Pow; }
        }

        protected override string Format
        {
            get { return "{0}^{1}"; }
        }
    }

    public class LogExpression : BinarySysExpression
    {
        public LogExpression(ISystemExpression op1, ISystemExpression op2) : base(op1, op2)
        {}

        protected override Func<double, double, double> F
        {
            get { return Math.Log; }
        }

        protected override string Format
        {
            get { return "log({0}, {1})"; }
        }
    }

    public abstract class UnarySysExpression : ISystemExpression
    {
        private readonly ISystemExpression _op1;

        protected UnarySysExpression(ISystemExpression op1)
        {
            _op1 = op1;
        }

        protected abstract Func<double, double> F { get; }
        protected abstract string Format { get; }

        public Delegate GetDelegate()
        {
            if (_op1.ParameterCount == 0) return (Func<double>) (() => F(((Func<double>)_op1.GetDelegate())()));
            return (Func<double, double>)(d => F(((Func<double, double>)_op1.GetDelegate())(d)));
        }

        public Expression GetParameterizedExpression()
        {
            return null;
        }

        public int ParameterCount
        {
            get { return _op1.ParameterCount; }
        }

        public override string ToString()
        {
            return string.Format(Format, _op1);
        }
    }

    public class SinExpression : UnarySysExpression
    {
        public SinExpression(ISystemExpression op1) : base(op1)
        {
        }

        protected override Func<double, double> F { get { return Math.Sin; } }
        protected override string Format { get { return "sin({0})"; } }
    }

    public abstract class BinarySysExpression : ISystemExpression
    {
        private readonly ISystemExpression _op1;
        private readonly ISystemExpression _op2;

        protected BinarySysExpression(ISystemExpression op1, ISystemExpression op2)
        {
            _op1 = op1;
            _op2 = op2;
        }

        protected abstract Func<double, double, double> F { get; }
        protected abstract string Format { get; }
        
        public Delegate GetDelegate()
        {
            var dVals = new List<double>();
            if (_op1.ParameterCount == 0) dVals.Add(((Func<double>)_op1.GetDelegate())());
            if (_op2.ParameterCount == 0) dVals.Add(((Func<double>)_op2.GetDelegate())());

            if (_op1.ParameterCount == 0 && _op2.ParameterCount == 0) return (Func<double>)(() => F(dVals[0], dVals[1]));
            if (_op1.ParameterCount == 0 && _op2.ParameterCount == 1)
                return (Func<double, double>) (d => F(dVals[0], ((Func<double, double>)_op2.GetDelegate())(d)));
            if (_op1.ParameterCount == 1 && _op2.ParameterCount == 0)
                return (Func<double, double>) (d => F(((Func<double, double>)_op1.GetDelegate())(d), dVals[0]));
            return (Func<double, double, double>) ((d0, d1) => F(((Func<double, double>)_op1.GetDelegate())(d0),
                ((Func<double, double>)_op2.GetDelegate())(d1)));
        }

        public Expression GetParameterizedExpression()
        {
            return null;
        }

        public int ParameterCount
        {
            get { return _op1.ParameterCount + _op2.ParameterCount; }
        }

        public override string ToString()
        {
            return string.Format("(" + Format + ")", _op1, _op2);
        }
    }

    public interface ISystemExpression : IExpression
    {
        Delegate GetDelegate();
        Expression GetParameterizedExpression();
        int ParameterCount { get; }
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

    public static class VIMExpression
    {
        private static readonly Dictionary<string, IExpression> _expressions = new Dictionary<string, IExpression>
                                                                          {
                                                                              {"pow", StackOpExpression.Power},
                                                                              {"log", StackOpExpression.Log},
                                                                              {"sin", StackOpExpression.Sin},
                                                                              {"cos", new UnaryMathExpression(d => Math.Cos(d))},
                                                                              {"tan", new UnaryMathExpression(d => Math.Tan(d))},
                                                                              {"asin", new UnaryMathExpression(d => Math.Asin(d))},
                                                                              {"acos", new UnaryMathExpression(d => Math.Acos(d))},
                                                                              {"atan", new UnaryMathExpression(d => Math.Atan(d))},
                                                                              {"pi", new DoubleExpression(Math.PI)},
                                                                              {"e", new DoubleExpression(Math.E)},
                                                                              {"reset", new NullExpression(Reset)},
                                                                              {"eval", StackOpExpression.EvalExp},
                                                                              {"sto", new StoExpression()},
                                                                              {"stoa", new StoExpression()},
                                                                              {"cmd", new StoExpression()},
                                                                              {"del", StackOpExpression.Delete},
                                                                              {"swap", StackOpExpression.Swap},
                                                                              {"graph", StackOpExpression.Graph}
                                                                          };

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
            var fn = sysExpr.GetDelegate();

            VIMMessageService.SendMessage((Action<IVIMGraphPanel>)(a => a.Graph(fn)));
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

    public interface IVIMStack
    {
        IExpression Pop();
        void Push(IExpression expression);
        int Count { get; }
    }
}