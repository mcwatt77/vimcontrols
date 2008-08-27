using System.Linq;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using VIMControls.Controls;

namespace VIMControls
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

    public class VIMFormControl : StackPanel, IVIMCharacterController, IVIMPersistable
    {
        public int _editingFieldIndex;
        public List<VIMFormRow> _rows;

        public VIMFormControl(IVIMControlContainer container)
        {
            var row = new VIMFormRow(this);
            _rows = new List<VIMFormRow> {row};
            Children.Add(row);

            Orientation = Orientation.Vertical;
            VerticalAlignment = VerticalAlignment.Top;

            container.EnterInsertMode(CharacterInsertMode.Before);
        }

        public void Output(char c)
        {
            var row = _rows[_editingFieldIndex];
            row.Output(c);
        }

        public void NewLine()
        {
            var row = new VIMFormRow(this);
            Children.Add(row);
            _rows.Add(row);
            _editingFieldIndex++;
        }

        public void Backspace()
        {
            _rows[_editingFieldIndex].Backspace();
        }

        public void Save()
        {
            Children
                .OfType<VIMFormRow>()
                .Select(row => new KeyValuePair<string, string>(row.FieldName.Value, row.Value.Value))
                .Persist();
        }
    }
}
