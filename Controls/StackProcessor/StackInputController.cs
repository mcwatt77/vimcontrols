using System;
using VIMControls.Contracts;
using VIMControls.Controls.Misc;

namespace VIMControls.Controls
{
    public interface IStackInputController : IVIMCharacterController, IVIMControl
    {}

    public class StackInputController : VIMTextControl, IStackInputController
    {
        public StackInputController()
        {
            ApplyBorders = true;
        }

        public new void NewLine()
        {
            VIMMessageService.SendMessage<IVIMExpressionProcessor>(a => a.Process(new VIMExpression(Text)));
            Text = String.Empty;
        }
    }
}