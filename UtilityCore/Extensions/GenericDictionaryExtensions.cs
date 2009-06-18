using System.Collections.Generic;

namespace Utility.Core
{
    public static class GenericDictionaryExtensions
    {
        public static TValue GetOrNull<TKey, TValue>(this IDictionary<TKey, TValue> src, TKey key) where TValue : class
        {
            return src.ContainsKey(key) ? src[key] : null;
        }

        public static TValue GetOrNew<TKey, TValue>(this IDictionary<TKey, TValue> src, TKey key) where TValue : new()
        {
            if (!src.ContainsKey(key))
                src[key] = new TValue();
            return src[key];
        }
    }
}