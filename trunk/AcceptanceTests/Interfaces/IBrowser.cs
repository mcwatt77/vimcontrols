using System.Collections.Generic;
using AcceptanceTests.Interfaces;

namespace AcceptanceTests.Interfaces
{
    public interface IBrowser : IView
    {
        IEnumerable<IBrowseElement> Elements { get; }
    }
}