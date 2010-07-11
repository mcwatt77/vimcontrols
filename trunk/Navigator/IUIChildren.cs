using System.Collections.Generic;
using Navigator.UI;

namespace Navigator
{
    public interface IUIChildren
    {
        IEnumerable<IUIElement> UIElements { get; }
    }
}