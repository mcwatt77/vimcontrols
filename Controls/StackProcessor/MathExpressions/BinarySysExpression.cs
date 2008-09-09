using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace VIMControls.Controls.StackProcessor.MathExpressions
{
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
            var op1Count = _op1.ParameterList.Count();
            var op2Count = _op2.ParameterList.Count();

            var dVals = new List<double>();
            if (op1Count == 0) dVals.Add(((Func<double>)_op1.GetDelegate())());
            if (op2Count == 0) dVals.Add(((Func<double>)_op2.GetDelegate())());

            if (op1Count == 0 && op2Count == 0) return (Func<double>)(() => F(dVals[0], dVals[1]));
            if (op1Count == 0 && op2Count == 1)
                return (Func<double, double>) (d => F(dVals[0], ((Func<double, double>)_op2.GetDelegate())(d)));
            if (op1Count == 1 && op2Count == 0)
                return (Func<double, double>) (d => F(((Func<double, double>)_op1.GetDelegate())(d), dVals[0]));
            if (ParameterList.Count() == 1)
                return (Func<double, double>) (d => F(((Func<double, double>) _op1.GetDelegate())(d),
                                                      ((Func<double, double>) _op2.GetDelegate())(d)));
            return (Func<double, double, double>) ((d0, d1) => F(((Func<double, double>)_op1.GetDelegate())(d0),
                                                                 ((Func<double, double>)_op2.GetDelegate())(d1)));
        }

        public Expression GetParameterizedExpression()
        {
            return null;
        }

        public IEnumerable<string> ParameterList
        {
            get { return _op1.ParameterList.Union(_op2.ParameterList); }
        }

        public override string ToString()
        {
            return string.Format("(" + Format + ")", _op1, _op2);
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
}