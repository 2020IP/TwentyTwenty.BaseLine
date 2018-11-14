using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using TwentyTwenty.BaseLine;

namespace System.Linq
{
    public static class EnumerableExtensions
    {
        public static bool SafeSequenceEqual<T>(this IEnumerable<T> first, IEnumerable<T> second)
        {
            var isEqual = false;

            if (first == null && second == null)
            {
                isEqual = true;
            }
            else if (first != null && second != null)
            {
                if (first.SequenceEqual(second))
                {
                    isEqual = true;
                }
            }

            return isEqual;
        }
        
        /// <summary>
        /// Adds an item to a list if it doesn't already contain the item.
        /// </summary>
        /// <typeparam name="T">The type of the items in the list</typeparam>
        /// <param name="list">The list to modify</param>
        /// <param name="value">The item to be added to the list.</param>
        public static void Fill<T>(this IList<T> list, T value)
        {
            if (list.Contains(value))
            {
                return;
            }

            list.Add(value);
        }

        /// <summary>
        /// Adds items to a list if the list doesn't already contain the item.
        /// </summary>
        /// <typeparam name="T">The type of the items in the list</typeparam>
        /// <param name="list">The list to modify</param>
        /// <param name="values">The items to be added to the list.</param>
        public static void Fill<T>(this IList<T> list, IEnumerable<T> values)
        {           
            list.AddRange(values.Except(list));
        }

        /// <summary>
        /// Removes all of the items that match the provided condition
        /// </summary>
        /// <typeparam name="T">The type of the items in the list</typeparam>
        /// <param name="list">The list to modify</param>
        /// <param name="whereEvaluator">The test to determine if an item should be removed</param>
        public static void RemoveAll<T>(this IList<T> list, Func<T, bool> whereEvaluator)
        {
            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (whereEvaluator(list[i]))
                {
                    list.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Concatenates a string between each item in a list of strings
        /// </summary>
        /// <param name="values">The array of strings to join</param>
        /// <param name="separator">The value to concatenate between items</param>
        /// <returns></returns>
        public static string Join(this string[] values, string separator)
        {
            return string.Join(separator, values);
        }

        /// <summary>
        /// Concatenates a string between each item in a sequence of strings
        /// </summary>
        /// <param name="values"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string Join(this IEnumerable<string> values, string separator)
        {
            return Join(values.ToArray(), separator);
        }

        /// <summary>
        /// Performs an action with a counter for each item in a sequence and provides
        /// </summary>
        /// <typeparam name="T">The type of the items in the sequence</typeparam>
        /// <param name="values">The sequence to iterate</param>
        /// <param name="eachAction">The action to performa on each item</param>
        /// <returns></returns>
        public static IEnumerable<T> Each<T>(this IEnumerable<T> values, Action<T, int> eachAction)
        {
            int index = 0;
            foreach (T item in values)
            {
                eachAction(item, index++);
            }

            return values;
        }

        //[DebuggerStepThrough]
        public static IEnumerable<T> Each<T>(this IEnumerable<T> values, Action<T> eachAction)
        {
            foreach (T item in values)
            {
                eachAction(item);
            }

            return values;
        }

        //[DebuggerStepThrough]
        public static IEnumerable Each(this IEnumerable values, Action<object> eachAction)
        {
            foreach (object item in values)
            {
                eachAction(item);
            }

            return values;
        }

        public static bool IsEqualTo<T>(this IEnumerable<T> actual, IEnumerable<T> expected)
        {
            var actualList = actual.ToArray();
            var expectedList = expected.ToArray();

            if (actualList.Length != expectedList.Length)
            {
                return false;
            }

            for (int i = 0; i < actualList.Length; ++i)
            {
                if (!actualList[i].Equals(expectedList[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public static IList<T> AddMany<T>(this IList<T> list, params T[] items)
        {
            return list.AddRange(items);
        }

        /// <summary>
        /// Appends a sequence of items to an existing list
        /// </summary>
        /// <typeparam name="T">The type of the items in the list</typeparam>
        /// <param name="list">The list to modify</param>
        /// <param name="items">The sequence of items to add to the list</param>
        /// <returns></returns>
        public static IList<T> AddRange<T>(this IList<T> list, IEnumerable<T> items)
        {
            items.Each(list.Add);
            return list;
        }

        public static IEnumerable<T> UnionWith<T>(this IEnumerable<T> first, params T[] second)
        {
            return first.Union(second);
        }

        public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> enumerable)
        {
            return enumerable ?? Enumerable.Empty<T>();
        }

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
        {
            return enumerable == null || !enumerable.Any();
        }

        public static IEnumerable<string> Trim(this IList<string> list)
        {
            int start = 0, end = list.Count - 1;
            while (start < end && string.IsNullOrWhiteSpace(list[start])) start++;
            while (end >= start && string.IsNullOrWhiteSpace(list[end])) end--;
            return list.Skip(start).Take(end - start + 1);
        }
    }
}