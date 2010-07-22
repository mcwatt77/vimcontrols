using System.Collections.Generic;
using VIControls.Commands.Interfaces;

namespace Navigator
{
    public interface IUIChildren
    {
        IEnumerable<IUIElement> UIElements { get; }
    }
}