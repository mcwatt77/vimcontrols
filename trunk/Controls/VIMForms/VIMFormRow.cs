using System.Windows;
using System.Windows.Controls;
using VIMControls.Contracts;

namespace VIMControls.Controls.VIMForms
{
    public class VIMFormRow : StackPanel, IVIMCharacterController
    {
        private readonly IVIMCharacterController _parent;
        public bool EditingFieldName { get; set; }

        public VIMTextControl FieldName { get; set; }
        public VIMTextControl Value { get; set; }

        public VIMFormRow(IVIMCharacterController parent)
        {
            _parent = parent;
            EditingFieldName = true;

            Orientation = Orientation.Horizontal;
            VerticalAlignment = VerticalAlignment.Top;

            FieldName =  new VIMTextControl {Width = 100, Height = 30};
            Children.Add(FieldName);

            Value = new VIMTextControl {Width = 100, Height = 30};
            Children.Add(Value);
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
    }
}