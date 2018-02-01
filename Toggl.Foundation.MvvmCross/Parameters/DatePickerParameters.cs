using System;
using Toggl.Multivac;

namespace Toggl.Foundation.MvvmCross.Parameters
{
    [Preserve(AllMembers = true)]
    public sealed class DatePickerParameters
    {
        public PickerMode Mode { get; set; }

        public DateTimeOffset CurrentDate { get; set; }

        public DateTimeOffset MinDate { get; set; }

        public DateTimeOffset MaxDate { get; set; }

        public static DatePickerParameters WithDates(PickerMode mode, DateTimeOffset current, DateTimeOffset min, DateTimeOffset max) =>
            new DatePickerParameters
            {
                Mode = mode,
                CurrentDate = current,
                MinDate = min,
                MaxDate = max
            };

        public enum PickerMode
        {
            Date,
            Time,
            DateTime
        }
    }
}
