using System.Collections.Generic;

namespace UITemplateViewer.Element
{
    public interface IEntityList : IEntityList<IEntityRow>
    {}

    public interface IEntityList<TRow>
        where TRow : IEntityRow
    {
        string DisplayName { get; set; }
        IEnumerable<TRow> Rows { get; set; }
    }
}