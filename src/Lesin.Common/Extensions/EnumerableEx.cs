using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Leisn.Common
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

    }
}
