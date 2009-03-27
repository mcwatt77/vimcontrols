using System.Collections.Generic;
using System.Linq;
using UITemplateViewer.Element;
using UITemplateViewer.WPF;

namespace UITemplateViewer
{
    public class EntitySelectorWrapper : IEntitySelector
    {
        private readonly EntitySelector _selector;

        public EntitySelectorWrapper(EntitySelector selector)
        {
            _selector = selector;
        }

        public IEnumerable<IEntityRow> Rows
        {
            get
            {
                return _selector.Rows.Cast<IEntityRow>();
            }
            set
            {
                _selector.Rows = value.Cast<IUIEntityRow>();
            }
        }

        public IEntityRow SelectedRow
        {
            get
            {
                return _selector.SelectedRow;
            }
            set
            {
                _selector.SelectedRow = (IUIEntityRow)value;
            }
        }
    }
}