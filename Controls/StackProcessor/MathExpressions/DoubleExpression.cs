using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace VIMControls.Controls.StackProcessor.MathExpressions
{
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

        public IEnumerable<string> ParameterList
        {
            get { return new List<string>(); }
        }
    }
}