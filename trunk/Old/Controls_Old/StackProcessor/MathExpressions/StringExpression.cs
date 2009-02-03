namespace VIMControls.Controls.StackProcessor.MathExpressions
{
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
}