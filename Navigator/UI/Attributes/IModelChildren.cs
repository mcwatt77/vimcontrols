using System.Collections.Generic;

namespace Navigator.UI.Attributes
{
    public interface IModelChildren : IAttribute
    {
        IEnumerable<object> Children { get; }
    }
}