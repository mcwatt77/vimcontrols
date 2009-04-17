using System;
using System.Linq.Expressions;

namespace UITemplateViewer.DynamicPath
{
    public class DecodedExpression
    {
        private readonly LambdaExpression _expression;
        private readonly Delegate _delegate;

        public bool Local { get; set; }

        public DecodedExpression(LambdaExpression expression)
        {
            _expression = expression;
            _delegate = _expression.Compile();
        }

        public object Invoke(params object[] args)
        {
            return _delegate.DynamicInvoke(args);
        }

        public override string ToString()
        {
            return _expression.ToString();
        }
    }
}