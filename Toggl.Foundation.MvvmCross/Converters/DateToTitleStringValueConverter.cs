using System;
using System.Globalization;
using MvvmCross.Platform.Converters;
using Toggl.Multivac;

namespace Toggl.Foundation.MvvmCross.Converters
{
    [Preserve(AllMembers = true)]
    public sealed class DateToTitleStringValueConverter : MvxValueConverter<DateTimeOffset, string>
    {
        protected override string Convert(DateTimeOffset value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.Date == DateTime.Now.Date)
                return Resources.Today;
            
            if (value.Date.AddDays(1) == DateTime.Now.Date)
                return Resources.Yesterday;

            return $"{value:ddd, dd MMM}";
        }
    }
}
