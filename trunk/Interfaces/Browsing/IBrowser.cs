using System.Collections.Generic;
using VIMControls.Interfaces;

namespace VIMControls.Interfaces
{
    public interface IBrowser : IView
    {
        IEnumerable<IBrowseElement> Elements { get; }
    }
}