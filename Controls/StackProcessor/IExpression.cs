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
    }

    public interface INumericExpression : IExpression
    {
        double dVal { get; }
    }

    public interface IFuncExpression : IExpression
    {
        IExpression Eval(IEnumerable<IExpression> args);
        int StackArgs { get; }
    }

    public abstract class MathExpression<TFunc> : IFuncExpression, IArgAggregateVisitor
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

        public T VisitArgAggregate<T>(T seed, Func<T, IExpression, T> fn)
        {
            return _args.Aggregate(seed, fn);
        }
    }

    public interface IArgAggregateVisitor
    {
        T VisitArgAggregate<T>(T seed, Func<T, IExpression, T> fn);
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

    public class BinaryMathExpression : MathExpression<Func<double, double, double>>
    {
        public BinaryMathExpression(Expression<Func<double, double, double>> fn) : base(fn)
        {
        }

        protected override double MathEval(IEnumerable<INumericExpression> args)
        {
            return _fn.Compile()(args.Last().dVal, args.First().dVal);
        }

        public override string ToString()
        {
            if (_fn.Body.NodeType == ExpressionType.Add)
                return "(" + _args.First() + " + " + _args.Last() + ")";
            if (_fn.Body.NodeType == ExpressionType.Multiply)
                return "(" + _args.First() + " * " + _args.Last() + ")";
            if (_fn.Body.NodeType == ExpressionType.Divide)
                return "(" + _args.First() + " / " + _args.Last() + ")";
            if (_fn.Body.NodeType == ExpressionType.Subtract)
                return "(" + _args.First() + " - " + _args.Last() + ")";
            if (_fn.Body.NodeType == ExpressionType.Call)
            {
                var method = _fn.Body as MethodCallExpression;
                return method.Method.Name + "(" + _args.First() + ", " + _args.Last() + ")";
            }
            return "f(" + _args.First() + ", " + _args.Last() + ")";
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

    public interface IEvalExpression : IFuncExpression
    {
        void SetStackArgs(IExpression arg);
    }

    public class EvalExpression : IEvalExpression
    {
        public IExpression Eval(IEnumerable<IExpression> args)
        {
            return new DoubleExpression(0);
        }

        public int StackArgs { get; private set; }

        public void SetStackArgs(IExpression arg)
        {
            StackArgs = 1;
        }
    }

    public static class ExpressionVisitor
    {
        public static void BinaryMath(BinaryMathExpression expr)
        {
        }
    }

    public static class VIMExpression
    {
        private static readonly Dictionary<string, IExpression> _expressions = new Dictionary<string, IExpression>
                                                                          {
                                                                              {"pow", new BinaryMathExpression((d0, d1) => Math.Pow(d0, d1))},
                                                                              {"log", new BinaryMathExpression((d0, d1) => Math.Log(d0, d1))},
                                                                              {"sin", new UnaryMathExpression(d => Math.Sin(d))},
                                                                              {"cos", new UnaryMathExpression(d => Math.Cos(d))},
                                                                              {"tan", new UnaryMathExpression(d => Math.Tan(d))},
                                                                              {"asin", new UnaryMathExpression(d => Math.Asin(d))},
                                                                              {"acos", new UnaryMathExpression(d => Math.Acos(d))},
                                                                              {"atan", new UnaryMathExpression(d => Math.Atan(d))},
                                                                              {"pi", new DoubleExpression(Math.PI)},
                                                                              {"e", new DoubleExpression(Math.E)},
                                                                              {"reset", new NullExpression(Reset)},
                                                                              {"eval", new EvalExpression()},
                                                                              {"sto", new StoExpression()},
                                                                              {"stoa", new StoExpression()},
                                                                              {"cmd", new StoExpression()}
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
}