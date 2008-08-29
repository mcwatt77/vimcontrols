using System.Linq;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace VIMControls.Controls.VIMForms
{
    public class VIMFormControl : StackPanel, IVIMCharacterController, IVIMPersistable, IVIMNavigable<List<KeyValuePair<string, string>>>, IVIMControl
    {
        public int _editingFieldIndex;
        public List<VIMFormRow> _rows;

        public VIMFormControl(IVIMActionController container)
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
                .Persist(VIMMRUControl.Guid);
        }

        public void Navigate(List<KeyValuePair<string, string>> obj)
        {
            Children.Clear();
            obj
                .Select(row =>
                            {
                                var vimRow = new VIMFormRow(this);
                                vimRow.FieldName.Text = row.Key;
                                vimRow.Value.Text = row.Value;
                                return vimRow;
                            })
                .Do(vimRow => Children.Add(vimRow));
        }

        public IUIElement GetUIElement()
        {
            return new UIElementWrapper(this);
        }
    }
}