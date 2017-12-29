using System;
using System.Globalization;
using MvvmCross.Platform.Converters;
using Toggl.Multivac;

namespace Toggl.Foundation.MvvmCross.Converters
{
    public sealed class EmailToStringConverter : MvxValueConverter<Email, string>
    {
        protected override string Convert(Email value, Type targetType, object parameter, CultureInfo culture)
            => value.ToString();

        protected override Email ConvertBack(string value, Type targetType, object parameter, CultureInfo culture)
            => Email.FromString(value);
    }
}
