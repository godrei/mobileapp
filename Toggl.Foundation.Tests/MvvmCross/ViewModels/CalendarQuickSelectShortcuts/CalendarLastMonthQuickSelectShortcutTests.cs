using System;
using Toggl.Foundation.MvvmCross.ViewModels.Calendar.QuickSelectShortcuts;

namespace Toggl.Foundation.Tests.MvvmCross.ViewModels.CalendarQuickSelectShortcuts
{
    public sealed class CalendarLastMonthQuickSelectShortcutTests
        : BaseCalendarQuickSelectShortcutTests<CalendarLastMonthQuickSelectShortcut>
    {
        protected override DateTimeOffset CurrentTime => new DateTimeOffset(2016, 4, 4, 1, 2, 3, TimeSpan.Zero);
        protected override DateTime ExpectedStart => new DateTime(2016, 3, 1);
        protected override DateTime ExpectedEnd => new DateTime(2016, 3, 31);

        protected override CalendarLastMonthQuickSelectShortcut CreateQuickSelectShortCut()
            => new CalendarLastMonthQuickSelectShortcut(TimeService);

        protected override CalendarLastMonthQuickSelectShortcut TryToCreateQuickSelectShortCutWithNull()
            => new CalendarLastMonthQuickSelectShortcut(null);
    }
}
