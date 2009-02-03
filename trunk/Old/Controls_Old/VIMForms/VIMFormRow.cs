using System.Windows;
using System.Windows.Controls;
using VIMControls.Contracts;
using VIMControls.Controls.Misc;

namespace VIMControls.Controls.VIMForms
{
    public class VIMFormRow : StackPanel, IVIMCharacterController
    {
        private readonly IVIMCharacterController _parent;
        private readonly IVIMFormConstraint _constraint;
        public bool EditingFieldName { get; set; }

        public VIMTextControl FieldName { get; set; }
        public VIMTextControl Value { get; set; }

        public VIMFormRow(IVIMCharacterController parent, IVIMFormConstraint constraint)
        {
            _parent = parent;
            _constraint = constraint;
            EditingFieldName = true;

            Orientation = Orientation.Horizontal;
            VerticalAlignment = VerticalAlignment.Top;

            FieldName =  new VIMTextControl {Width = 100, Height = 30};
            var fnUiElement = UIElementWrapper.From(FieldName);
            Children.Add(fnUiElement);

            Value = new VIMTextControl {Width = 100, Height = _constraint.Multiline ? 100 : 30};
            var vUiElement = UIElementWrapper.From(Value);
            Children.Add(vUiElement);

            FieldName.ApplyBorders = true;
            Value.ApplyBorders = true;
        }

        public void Output(char c)
        {
            if (EditingFieldName && c == '\t')
            {
                EditingFieldName = false;
                return;
            }
            if (c == '\t')
            {
                _parent.NewLine();
                return;
            }

            if (EditingFieldName)
                FieldName.Output(c);
            else
                Value.Output(c);
        }

        public void NewLine()
        {
        }

        public void Backspace()
        {
            if (EditingFieldName)
                FieldName.Backspace();
            else
                Value.Backspace();
        }

        public IUIElement GetUIElement()
        {
            return new UIElementWrapper(this);
        }

        public void ResetInput()
        {
        }

        public void MissingModeAction(IVIMAction action)
        {
        }

        public void MissingMapping()
        {
        }
    }
}