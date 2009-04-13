using System.Collections.Generic;
using NodeMessaging;

namespace UITemplateViewer.Element
{
    public interface IEntityRow
    {
        IParentNode Context { get; set; }
        IEnumerable<IAccessor<string>> Columns { get; set; }
    }
}