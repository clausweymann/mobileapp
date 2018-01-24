using System;
using Toggl.Foundation.MvvmCross.ViewModels.Calendar.QuickSelectShortcuts;

namespace Toggl.Foundation.Tests.MvvmCross.ViewModels.CalendarQuickSelectShortcuts
{
    public sealed class CalendarTodayQuickSelectShortcutTests
        : BaseCalendarQuickSelectShortcutTests<CalendarTodayQuickSelectShortcut>
    {
        protected override DateTimeOffset CurrentTime => new DateTimeOffset(2020, 1, 2, 3, 4, 5, TimeSpan.Zero);
        protected override DateTime ExpectedStart => new DateTime(2020, 1, 2);
        protected override DateTime ExpectedEnd => new DateTime(2020, 1, 2);

        protected override CalendarTodayQuickSelectShortcut CreateQuickSelectShortcut()
            => new CalendarTodayQuickSelectShortcut(TimeService);

        protected override CalendarTodayQuickSelectShortcut TryToCreateQuickSelectShortCutWithNull()
            => new CalendarTodayQuickSelectShortcut(null);
    }
}
