using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace VIMControls.Controls.StackProcessor.MathExpressions
{
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

        public IEnumerable<string> ParameterList
        {
            get { return new List<string> {_name}; }
        }

        public override string ToString()
        {
            return _name;
        }
    }
}