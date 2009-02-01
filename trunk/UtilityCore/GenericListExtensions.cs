using System;
using System.Collections.Generic;

namespace Utility.Core
{
    public static class GenericListExtensions
    {
        public static int ReverseFindIndex<T>(this List<T> src, Predicate<T> fn)
        {
            for (var i = src.Count - 1; i >= 0; i--)
                if (fn(src[i])) return i;

            return -1;
        }
    }
}