using System.Collections.Generic;
using UITemplateViewer.Element;

namespace UITemplateViewer.Element
{
    public interface IEntitySelector : IEntitySelector<IEntityRow>
    {}

    public interface IEntitySelector<TRow>
        where TRow : IEntityRow
    {
        IEnumerable<TRow> Rows { get; set; }
        TRow SelectedRow { get; set; }
    }
}