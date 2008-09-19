using System.Collections.Generic;
using VIMControls.Interfaces;
using VIMControls.Interfaces.Graphing;

namespace VIMControls.Interfaces.Graphing
{
    public interface IGraphView : IView
    {
        IEnumerable<IGraphable> DisplayedObjects { get; }
    }
}