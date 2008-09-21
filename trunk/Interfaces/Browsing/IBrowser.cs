using System.Collections.Generic;
using VIMControls.Input;
using VIMControls.Interfaces;

namespace VIMControls.Interfaces
{
    public interface IBrowser : IView, IMovable, INavigable
    {
        IEnumerable<IBrowseElement> Elements { get; }
    }
}