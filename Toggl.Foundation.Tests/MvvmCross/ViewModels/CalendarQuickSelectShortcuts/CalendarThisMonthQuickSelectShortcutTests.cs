using System;
using Toggl.Foundation.MvvmCross.ViewModels.Calendar.QuickSelectShortcuts;

namespace Toggl.Foundation.Tests.MvvmCross.ViewModels.CalendarQuickSelectShortcuts
{
    public sealed class CalendarThisMonthQuickSelectShortcutTests
        : BaseCalendarQuickSelectShortcutTests<CalendarThisMonthQuickSelectShortcut>
    {
        protected override DateTimeOffset CurrentTime => new DateTimeOffset(2017, 11, 28, 0, 0, 0, TimeSpan.Zero);
        protected override DateTime ExpectedStart => new DateTime(2017, 11, 1);
        protected override DateTime ExpectedEnd => new DateTime(2017, 11, 30);

        protected override CalendarThisMonthQuickSelectShortcut CreateQuickSelectShortCut()
            => new CalendarThisMonthQuickSelectShortcut(TimeService);

        protected override CalendarThisMonthQuickSelectShortcut TryToCreateQuickSelectShortCutWithNull()
            => new CalendarThisMonthQuickSelectShortcut(null);
    }
}
