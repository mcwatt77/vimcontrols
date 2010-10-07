using System.Collections.Generic;

namespace Navigator.UI.Attributes
{
    public interface IHasRows<TTarget>
        where TTarget : IHasColumns<TTarget>
    {
        IEnumerable<TTarget> Children { get; }
    }
}