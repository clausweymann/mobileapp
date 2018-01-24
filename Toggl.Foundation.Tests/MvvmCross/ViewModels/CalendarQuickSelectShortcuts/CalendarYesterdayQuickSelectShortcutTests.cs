using System;
using Toggl.Foundation.MvvmCross.ViewModels.Calendar.QuickSelectShortcuts;

namespace Toggl.Foundation.Tests.MvvmCross.ViewModels.CalendarQuickSelectShortcuts
{
    public sealed class CalendarYesterdayQuickSelectShortcutTests
        : BaseCalendarQuickSelectShortcutTests<CalendarYesterdayQuickSelectShortcut>
    {
        protected override DateTimeOffset CurrentTime => new DateTimeOffset(1998, 4, 5, 6, 4, 2, TimeSpan.Zero);
        protected override DateTime ExpectedStart => new DateTime(1998, 4, 4);
        protected override DateTime ExpectedEnd => new DateTime(1998, 4, 4);

        protected override CalendarYesterdayQuickSelectShortcut CreateQuickSelectShortcut()
            => new CalendarYesterdayQuickSelectShortcut(TimeService);

        protected override CalendarYesterdayQuickSelectShortcut TryToCreateQuickSelectShortCutWithNull()
            => new CalendarYesterdayQuickSelectShortcut(null);
    }
}
