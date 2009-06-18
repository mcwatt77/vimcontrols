using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utility.Core
{
    public static class GenericEnumerableExtensions
    {
        public static void Do<TSource>(this IEnumerable<TSource> src, Action<TSource> fn)
        {
            foreach (var item in src)
                fn(item);
        }
        
        public static void Do<TSource>(this IEnumerable<TSource> src, Action<int, TSource> fn)
        {
            var i = 0;
            foreach (var item in src)
                fn(i++, item);
        }

        public static bool CompareTo<TSource>(this IEnumerable<TSource> source, IEnumerable<TSource> comp)
        {
            IEnumerator<TSource> eSource = source.GetEnumerator(), eComp = comp.GetEnumerator();
            while (eSource.MoveNext())
            {
                if (!eComp.MoveNext())
                    return false;
                if (!eSource.Current.Equals(eComp.Current))
                    return false;
            }
            return !eComp.MoveNext();
        }

        public static IEnumerable<TItem> Flatten<TItem>(this IEnumerable<IEnumerable<TItem>> src)
        {
            foreach (var list in src)
                foreach (var item in list)
                    yield return item;
        }

        public static IEnumerable<T> Flush<T>(this IEnumerable<T> src)
        {
            return src.ToList();
        }

        public static string SeparateBy(this IEnumerable<string> src, string separator)
        {
            var e = src.GetEnumerator();
            if (!e.MoveNext()) return String.Empty;
            var sb = new StringBuilder(e.Current);
            while (e.MoveNext())
            {
                sb.Append(separator);
                sb.Append(e.Current);
            }
            return sb.ToString();
        }

        public static IEnumerable<int> FindAllIndexes<T>(this IEnumerable<T> src, Predicate<T> fn)
        {
            var i = 0;
            var e = src.GetEnumerator();
            while (e.MoveNext())
                if (fn(e.Current))
                    yield return i;
        }
    }
}