using System;
using System.Collections.Generic;

namespace Toggl.Multivac.Extensions
{
    public static class FunctionalExtensions
    {
        public static TResult Apply<T, TResult>(this T self, Func<T, TResult> funcToApply)
            => funcToApply(self);

        public static void ForEach<T>(this IEnumerable<T> self, Action<T> action)
        {
            Ensure.Argument.IsNotNull(self, nameof(self));
            Ensure.Argument.IsNotNull(action, nameof(action));

            foreach (T item in self)
            {
                action(item);
            }
        }

        public static IEnumerable<T> Do<T>(this IEnumerable<T> self, Action<T> action)
        {
            Ensure.Argument.IsNotNull(self, nameof(self));
            Ensure.Argument.IsNotNull(action, nameof(action));

            self.ForEach(action);
            return self;
        }
    }
}
