using System;
using System.Globalization;
using MvvmCross.Platform.Converters;
using Toggl.Multivac;

namespace Toggl.Foundation.MvvmCross.Converters
{
    public sealed class PasswordToStringValueConverter
        : MvxValueConverter<Password, string>
    {
        protected override string Convert(Password value, Type targetType, object parameter, CultureInfo culture)
            => value.ToString();

        protected override Password ConvertBack(string value, Type targetType, object parameter, CultureInfo culture)
            => Password.FromString(value);
    }
}
