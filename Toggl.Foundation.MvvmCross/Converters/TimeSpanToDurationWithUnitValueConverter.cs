using System;
using System.Globalization;
using System.Text;
using MvvmCross.Platform.Converters;
using Toggl.Multivac;

namespace Toggl.Foundation.MvvmCross.Converters
{
    [Preserve(AllMembers = true)]
    public sealed class TimeSpanToDurationWithUnitValueConverter : MvxValueConverter<TimeSpan, string>
    {
        protected override string Convert(TimeSpan value, Type targetType, object parameter, CultureInfo culture)
        {
            var unit = getUnitFor(value);
            var durationBuilder = new StringBuilder();

            if (value.TotalHours > 1)
                durationBuilder.Append(((int)value.TotalHours).ToString("00"))
                               .Append(":");
            if (value.TotalMinutes > 1)
                durationBuilder.Append(value.Minutes.ToString("00"))
                               .Append(":");
            durationBuilder.Append(value.Seconds.ToString("00"));
            
            if (!string.IsNullOrEmpty(unit))
                durationBuilder.Append($" {unit}");

            return durationBuilder.ToString();
        }

        private string getUnitFor(TimeSpan timeSpan)
        {
            if (timeSpan.TotalHours > 1)
                return "";
            if (timeSpan.Minutes > 0)
                return Resources.UnitMin;
            return Resources.UnitSecond;
        }
    }
}
