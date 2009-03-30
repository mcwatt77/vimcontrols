using System.Collections.Generic;
using UITemplateViewer.Element;
using UITemplateViewer.WPF;

namespace UITemplateViewer.WPF
{
    public class EntitySelector : IEntitySelector<IUIEntityRow>
    {
        private IUIEntityRow _selectedRow;

        public IEnumerable<IUIEntityRow> Rows { get; set; }
        public IUIEntityRow SelectedRow
        {
            get { return _selectedRow; }
            set
            {
                if (_selectedRow != null) _selectedRow.Selected = false;
                _selectedRow = value;
                _selectedRow.Selected = true;
            }
        }
    }
}