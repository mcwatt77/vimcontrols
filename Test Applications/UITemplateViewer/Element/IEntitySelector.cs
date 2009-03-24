using System.Collections.Generic;
using UITemplateViewer.Element;

namespace UITemplateViewer.Element
{
    public interface IEntitySelector
    {
        IEnumerable<IEntityRow> Rows { get; set; }
        IEntityRow SelectedRow { get; set; }
    }
}