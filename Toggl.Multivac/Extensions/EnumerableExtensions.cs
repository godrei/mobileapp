using System;
using System.Collections.Generic;

namespace Toggl.Multivac.Extensions
{
    public static class EnumerableExtensions
    {
        public static int IndexOf<T>(
            this IEnumerable<T> self, Func<T, bool> predicate)
        {
            int i = 0;
            foreach (var item in self)
            {
                if (predicate(item))
                    return i;
                i++;
            }
            return -1;
        }
    }
}
