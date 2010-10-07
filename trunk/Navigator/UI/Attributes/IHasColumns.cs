using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Navigator.UI.Attributes
{
    public interface IHasColumns<TTarget>
    {
        IEnumerable<Expression<Func<TTarget, string>>> Columns { get; }
    }
}