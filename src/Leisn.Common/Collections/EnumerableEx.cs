// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.Collections;
using System.Collections.Generic;

namespace Leisn.Common.Collections
{
    public static class EnumerableEx
    {

        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> self, Action<T> action)
        {
            foreach (T? item in self)
            {
                action?.Invoke(item);
            }
            return self;
        }

        public static IEnumerable ForEach(this IEnumerable self, Action<object> action)
        {
            foreach (object? item in self)
            {
                action?.Invoke(item);
            }
            return self;
        }

        /// <summary>
        /// Sub array,keep length with `size` or less, and keep `midIndex` in middle.
        /// </summary>
        /// <typeparam name="T">Type of array</typeparam>
        /// <param name="array">Source array</param>
        /// <param name="midIndex">The index of middle item</param>
        /// <param name="size">The max size of return array</param>
        /// <returns></returns>
        public static ArraySegment<T> SubMidIndex<T>(this T[] array, int midIndex, int size)
        {
            int start = Math.Max(0, midIndex - size / 2);
            int end = start + size;
            if (end > array.Length)
            {
                start -= end - array.Length;
                end = array.Length;
                start = Math.Max(0, start);
            }
            return new ArraySegment<T>(array, start, end - start);
        }

    }
}
