using System.Collections.Generic;
using VIMControls.Interfaces;

namespace VIMControls.Interfaces
{
    public interface IFormEditor : IView
    {
        IEnumerable<KeyValuePair<string, IExpression>> Data { get; }
    }
}