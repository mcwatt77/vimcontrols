using System.Collections.Generic;
using AcceptanceTests.Interfaces;

namespace AcceptanceTests.Interfaces
{
    public interface IFormEditor : IView
    {
        IEnumerable<KeyValuePair<string, IExpression>> Data { get; }
    }
}