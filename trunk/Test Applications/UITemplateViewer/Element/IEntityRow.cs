using System.Collections.Generic;
using NodeMessaging;

namespace UITemplateViewer.Element
{
    public interface IEntityRow
    {
        IEnumerable<IStringProvider> Columns { get; set; }
        bool Selected { get; set; }
    }
}