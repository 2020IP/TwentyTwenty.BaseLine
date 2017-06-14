using System.Collections.Generic;

namespace System.Collections.Concurrent
{
    public static class ConcurrentDictionaryExtensions
    {
        public static IEnumerable<TElement> ValuesWithoutLock<TKey, TElement>(this ConcurrentDictionary<TKey, TElement> source)
        {
            foreach (var item in source)
            {
                if (item.Value != null)
                {
                    yield return item.Value;
                }
            }
        }

        public static IEnumerable<TKey> KeysWithoutLock<TKey, TElement>(this ConcurrentDictionary<TKey, TElement> source)
        {
            foreach (var item in source)
            {
                yield return item.Key;
            }
        }
    }
}