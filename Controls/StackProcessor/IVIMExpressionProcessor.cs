using VIMControls.Contracts;
using VIMControls.Controls.Misc;

namespace VIMControls.Controls
{
    public interface IVIMExpressionProcessor : IVIMControl, IVIMMultiLineTextDisplay, IVIMController
    {
        void Process(IExpression expr);
    }

    public class ExpressionProcessor : VIMTextControl, IVIMExpressionProcessor
    {
        public ExpressionProcessor()
        {
            ApplyBorders = true;
        }
        public void Process(IExpression expr)
        {
            Text = expr.ToString();
        }
    }

    public class FancyDisplayStack : VIMTextControl, IFancyDisplayStack
    {
        public void Push(IExpression expr)
        {
            Text = expr.ToString();
        }
    }

    public interface IExpression
    {
    }

    public class VIMExpression : IExpression
    {
        private readonly string expr;

        public VIMExpression(string expr)
        {
            this.expr = expr;
        }

        public override string ToString()
        {
            return expr;
        }
    }

    public interface IFancyDisplayStack
    {
        void Push(IExpression expr);
    }
}