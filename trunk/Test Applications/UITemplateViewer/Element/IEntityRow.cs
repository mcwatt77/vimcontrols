using System.Collections.Generic;
using NodeMessaging;

namespace UITemplateViewer.Element
{
    public interface IEntityRow
    {
        IParentNode Context { get; set; }
        IEnumerable<IFieldAccessor<string>> Columns { get; set; }
    }
}