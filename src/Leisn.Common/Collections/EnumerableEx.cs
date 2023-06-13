// By Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.Collections;
using System.Collections.Generic;

namespace Leisn.Common.Collections
{
    public static class EnumerableEx
    {

        public static IEnumerable<T> ForEeach<T>(this IEnumerable<T> self, Action<T> action)
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

    }
}
