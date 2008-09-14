using System.Collections.Generic;
using AcceptanceTests.Interfaces;

namespace AcceptanceTests.Interfaces.Graphing
{
    public interface IGraphView : IView
    {
        IEnumerable<IGraphable> DisplayedObjects { get; }
    }
}