using System.Collections.Generic;
using UITemplateViewer.Element;
using UITemplateViewer.WPF;

namespace UITemplateViewer
{
    public class EntitySelector : IEntitySelector
    {
        private IEntityRow _selectedRow;

        public IEnumerable<IEntityRow> Rows { get; set; }
        public IEntityRow SelectedRow
        {
            get { return _selectedRow; }
            set
            {
                var selectedRow = (EntityRow) _selectedRow;
                if (_selectedRow != null) selectedRow.Selected = false;

                _selectedRow = value;
                selectedRow = (EntityRow) _selectedRow;
                selectedRow.Selected = true;
            }
        }
    }
}