using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace VIMControls.Controls.StackProcessor.MathExpressions
{
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
            if (_op1.ParameterList.Count() == 0) return (Func<double>) (() => F(((Func<double>)_op1.GetDelegate())()));
            return (Func<double, double>)(d => F(((Func<double, double>)_op1.GetDelegate())(d)));
        }

        public Expression GetParameterizedExpression()
        {
            if (_op1.ParameterList.Count() == 0) return (Expression<Func<double>>) (() => F(((Func<double>)_op1.GetDelegate())()));
            return (Expression<Func<double, double>>)(d => F(((Func<double, double>)_op1.GetDelegate())(d)));
        }

        public IEnumerable<string> ParameterList
        {
            get { return _op1.ParameterList; }
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

    public class CosExpression : UnarySysExpression
    {
        public CosExpression(ISystemExpression op1) : base(op1)
        {
        }

        protected override Func<double, double> F { get { return Math.Cos; } }
        protected override string Format { get { return "cos({0})"; } }
    }

    public class TanExpression : UnarySysExpression
    {
        public TanExpression(ISystemExpression op1) : base(op1)
        {
        }

        protected override Func<double, double> F { get { return Math.Tan; } }
        protected override string Format { get { return "tan({0})"; } }
    }

    public class ASinExpression : UnarySysExpression
    {
        public ASinExpression(ISystemExpression op1) : base(op1)
        {
        }

        protected override Func<double, double> F { get { return Math.Asin; } }
        protected override string Format { get { return "asin({0})"; } }
    }

    public class ACosExpression : UnarySysExpression
    {
        public ACosExpression(ISystemExpression op1) : base(op1)
        {
        }

        protected override Func<double, double> F { get { return Math.Acos; } }
        protected override string Format { get { return "acos({0})"; } }
    }

    public class ATanExpression : UnarySysExpression
    {
        public ATanExpression(ISystemExpression op1) : base(op1)
        {
        }

        protected override Func<double, double> F { get { return Math.Atan; } }
        protected override string Format { get { return "atan({0})"; } }
    }
}