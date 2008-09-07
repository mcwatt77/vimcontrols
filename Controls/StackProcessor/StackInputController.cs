using System;
using VIMControls.Contracts;
using VIMControls.Controls.Misc;
using VIMControls.Controls.StackProcessor;

namespace VIMControls.Controls.StackProcessor
{
    public interface IStackInputController : IVIMCharacterController
    {
        void Function(IFuncExpression expr);
    }

    public class StackInputController : VIMTextControl, IStackInputController
    {
        public StackInputController()
        {
            ApplyBorders = true;
        }

        public new void NewLine()
        {
            if (Text.Length == 0) return;

            double dVal;
            var expr = double.TryParse(Text, out dVal) ? new DoubleExpression(dVal) : VIMExpression.FromString(Text);
            Text = String.Empty;

            if (expr is IFuncExpression)
                VIMMessageService.SendMessage<IVIMExpressionProcessor>(a => a.Eval((IFuncExpression)expr));
            else
                VIMMessageService.SendMessage<IVIMExpressionProcessor>(a => a.Push(expr));
        }

        public void Function(IFuncExpression expr)
        {
            NewLine();
            VIMMessageService.SendMessage<IVIMExpressionProcessor>(a => a.Eval(expr));
        }
    }
}