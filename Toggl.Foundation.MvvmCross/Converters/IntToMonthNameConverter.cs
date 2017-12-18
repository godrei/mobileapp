using System;
using System.Collections.Generic;
using System.Globalization;
using MvvmCross.Platform.Converters;

namespace Toggl.Foundation.MvvmCross.Converters
{
    public sealed class IntToMonthNameConverter : MvxValueConverter<int, string>
    {
        private static readonly Dictionary<int, string> translations = new Dictionary<int, string>
        {
            { 1, Resources.January },
            { 2, Resources.February },
            { 3, Resources.March },
            { 4, Resources.April },
            { 5, Resources.May },
            { 6, Resources.June },
            { 7, Resources.July },
            { 8, Resources.August },
            { 9, Resources.September },
            { 10, Resources.October },
            { 11, Resources.November },
            { 12, Resources.December },
        };

        protected override string Convert(int value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value < 1 || value > 12)
                throw new ArgumentOutOfRangeException($"{nameof(value)} must be in range [1 - 12]");

            return translations[value];
        }
    }
}
