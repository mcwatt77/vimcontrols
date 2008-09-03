using System.Linq;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using VIMControls.Contracts;

namespace VIMControls.Controls.VIMForms
{
    public class VIMFormControl : StackPanel, IVIMForm
    {
        public int _editingFieldIndex;
        public List<VIMFormRow> _rows;
        private IVIMFormConstraint _defaultConstraint = VIMFormConstraint.Default;

        public VIMFormControl(IVIMActionController container)
        {
            var row = new VIMFormRow(this, _defaultConstraint);
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
            var row = new VIMFormRow(this, _defaultConstraint);
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
                .Persist(VIMMRUControl.Guid);
        }

        public void Delete()
        {
            MessageBox.Show("I'm scared to delete!");
        }

        public void Navigate(List<KeyValuePair<string, string>> obj)
        {
            Children.Clear();
            obj
                .Select(row =>
                            {
                                var vimRow = new VIMFormRow(this, _defaultConstraint)
                                                 {
                                                     FieldName = {Text = row.Key},
                                                     Value = {Text = row.Value}
                                                 };
                                return vimRow;
                            })
                .Do(vimRow => Children.Add(vimRow));
        }

        public void SetMode(IVIMFormConstraint constraint)
        {
            _defaultConstraint = constraint;
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