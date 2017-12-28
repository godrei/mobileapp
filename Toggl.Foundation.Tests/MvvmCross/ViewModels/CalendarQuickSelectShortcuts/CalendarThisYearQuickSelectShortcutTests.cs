using System;
using Toggl.Foundation.MvvmCross.ViewModels.Calendar.QuickSelectShortcuts;

namespace Toggl.Foundation.Tests.MvvmCross.ViewModels.CalendarQuickSelectShortcuts
{
    public sealed class CalendarThisYearQuickSelectShortcutTests
        : BaseCalendarQuickSelectShortcutTests<CalendarThisYearQuickSelectShortcut>
    {
        protected override DateTimeOffset CurrentTime => new DateTimeOffset(1984, 4, 5, 6, 7, 8, TimeSpan.Zero);
        protected override DateTime ExpectedStart => new DateTime(1984, 1, 1);
        protected override DateTime ExpectedEnd => new DateTime(1984, 12, 31);

        protected override CalendarThisYearQuickSelectShortcut CreateQuickSelectShortCut()
            => new CalendarThisYearQuickSelectShortcut(TimeService);

        protected override CalendarThisYearQuickSelectShortcut TryToCreateQuickSelectShortCutWithNull()
            => new CalendarThisYearQuickSelectShortcut(null);
    }
}
