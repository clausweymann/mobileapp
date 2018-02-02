using System;
using Toggl.Multivac;

namespace Toggl.Foundation.MvvmCross.Parameters
{
    [Preserve(AllMembers = true)]
    public sealed class DateTimePickerParameters
    {
        public DateTimePickerMode Mode { get; set; }

        public DateTimeOffset CurrentDate { get; set; }

        public DateTimeOffset MinDate { get; set; }

        public DateTimeOffset MaxDate { get; set; }

        public static DateTimePickerParameters WithDates(DateTimePickerMode mode, DateTimeOffset current, DateTimeOffset min, DateTimeOffset max)
        {
            if (min > max)
                throw new ArgumentException("Max date must be later than Min date.");

            return new DateTimePickerParameters
            {
                Mode = mode,
                CurrentDate = current,
                MinDate = min,
                MaxDate = max
            };
        }
    }

    public enum DateTimePickerMode
    {
        Date,
        Time,
        DateTime
    }
}
