using System.Collections.Generic;

namespace UITemplateViewer.Element
{
    public interface IEntityList
    {
        string DisplayName { get; set; }
        IEnumerable<IEntityRow> Rows { get; set; }
        IEntityRow SelectedRow { get; set; }
    }
}