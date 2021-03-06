﻿using System;
using System.Collections.Generic;
using System.Linq;

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

        public static IEnumerable<TRight> SelectAllRight<TLeft, TRight>(this IEnumerable<Either<TLeft, TRight>> self)
            => self.Where(either => either.IsRight).Select(either => either.Right);

        public static IEnumerable<TLeft> SelectAllLeft<TLeft, TRight>(this IEnumerable<Either<TLeft, TRight>> self)
            => self.Where(either => either.IsLeft).Select(either => either.Left);

        public static IEnumerable<T> SelectNonNulls<T>(this IEnumerable<Nullable<T>> self) where T : struct
            => self.Where(nullable => nullable.HasValue).Select(nullable => nullable.Value);
    }
}
