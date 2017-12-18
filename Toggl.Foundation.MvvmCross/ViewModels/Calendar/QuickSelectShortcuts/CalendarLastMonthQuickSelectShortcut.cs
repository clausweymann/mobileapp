﻿using System;
using Toggl.Foundation.MvvmCross.Parameters;

namespace Toggl.Foundation.MvvmCross.ViewModels.Calendar.QuickSelectShortcuts
{
    public sealed class CalendarLastMonthQuickSelectShortcut
        : CalendarBaseQuickSelectShortcut
    {
        public CalendarLastMonthQuickSelectShortcut(ITimeService timeService)
            : base(timeService, Resources.LastMonth)
        {
        }

        public override DateRangeParameter GetDateRange()
        {
            var lastMonth = TimeService.CurrentDateTime.Date.AddMonths(-1);
            var start = new DateTimeOffset(lastMonth.Year, lastMonth.Month, 1, 0, 0, 0, TimeSpan.Zero);
            var end = start.AddMonths(1).AddDays(-1);
            return DateRangeParameter.WithDates(start, end);
        }
    }
}
