using System;
using System.Globalization;
using MvvmCross.Platform.Converters;

namespace Toggl.Daneel.Converters
{
    public class ComplementValueConverter : MvxValueConverter<float, float>
    {
        protected override float Convert(float value, Type targetType, object parameter, CultureInfo culture)
            => Math.Abs(value - 1);

        protected override float ConvertBack(float value, Type targetType, object parameter, CultureInfo culture)
            => Math.Abs(value - 1);
    }
}
