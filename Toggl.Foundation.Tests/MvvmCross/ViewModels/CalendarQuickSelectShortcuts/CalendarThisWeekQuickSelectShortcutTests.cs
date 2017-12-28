using System;
using Toggl.Foundation.MvvmCross.ViewModels.Calendar.QuickSelectShortcuts;
using Toggl.Multivac;

namespace Toggl.Foundation.Tests.MvvmCross.ViewModels.CalendarQuickSelectShortcuts
{
    public abstract class CalendarThisWeekQuickSelectShortcutTests
            : BaseCalendarQuickSelectShortcutTests<CalendarThisWeekQuickSelectShortcut>
    {
        protected abstract BeginningOfWeek BeginningOfWeek { get; }

        protected sealed override CalendarThisWeekQuickSelectShortcut CreateQuickSelectShortCut()
            => new CalendarThisWeekQuickSelectShortcut(TimeService, BeginningOfWeek);

        protected sealed override CalendarThisWeekQuickSelectShortcut TryToCreateQuickSelectShortCutWithNull()
            => new CalendarThisWeekQuickSelectShortcut(null, BeginningOfWeek);

        public sealed class WhenBeginningOfWeekIsMonday
            : CalendarThisWeekQuickSelectShortcutTests
        {
            protected override BeginningOfWeek BeginningOfWeek => BeginningOfWeek.Monday;
            protected override DateTimeOffset CurrentTime => new DateTimeOffset(2017, 12, 26, 0, 0, 0, TimeSpan.Zero);
            protected override DateTime ExpectedStart => new DateTime(2017, 12, 25);
            protected override DateTime ExpectedEnd => new DateTime(2017, 12, 31);
        }

        public sealed class WhenBeginningOfWeekIsTuesday
           : CalendarThisWeekQuickSelectShortcutTests
        {
            protected override BeginningOfWeek BeginningOfWeek => BeginningOfWeek.Tuesday;
            protected override DateTimeOffset CurrentTime => new DateTimeOffset(2017, 12, 26, 0, 0, 0, TimeSpan.Zero);
            protected override DateTime ExpectedStart => new DateTime(2017, 12, 26);
            protected override DateTime ExpectedEnd => new DateTime(2018, 1, 1);
        }

        public sealed class WhenBeginningOfWeekIsWednesday
           : CalendarThisWeekQuickSelectShortcutTests
        {
            protected override BeginningOfWeek BeginningOfWeek => BeginningOfWeek.Wednesday;
            protected override DateTimeOffset CurrentTime => new DateTimeOffset(2017, 12, 26, 0, 0, 0, TimeSpan.Zero);
            protected override DateTime ExpectedStart => new DateTime(2017, 12, 20);
            protected override DateTime ExpectedEnd => new DateTime(2017, 12, 26);
        }

        public sealed class WhenBeginningOfWeekIsThursday
           : CalendarThisWeekQuickSelectShortcutTests
        {
            protected override BeginningOfWeek BeginningOfWeek => BeginningOfWeek.Thursday;
            protected override DateTimeOffset CurrentTime => new DateTimeOffset(2017, 12, 26, 0, 0, 0, TimeSpan.Zero);
            protected override DateTime ExpectedStart => new DateTime(2017, 12, 21);
            protected override DateTime ExpectedEnd => new DateTime(2017, 12, 27);
        }

        public sealed class WhenBeginningOfWeekIsFriday
           : CalendarThisWeekQuickSelectShortcutTests
        {
            protected override BeginningOfWeek BeginningOfWeek => BeginningOfWeek.Friday;
            protected override DateTimeOffset CurrentTime => new DateTimeOffset(2017, 12, 26, 0, 0, 0, TimeSpan.Zero);
            protected override DateTime ExpectedStart => new DateTime(2017, 12, 22);
            protected override DateTime ExpectedEnd => new DateTime(2017, 12, 28);
        }

        public sealed class WhenBeginningOfWeekIsSaturday
           : CalendarThisWeekQuickSelectShortcutTests
        {
            protected override BeginningOfWeek BeginningOfWeek => BeginningOfWeek.Saturday;
            protected override DateTimeOffset CurrentTime => new DateTimeOffset(2017, 12, 26, 0, 0, 0, TimeSpan.Zero);
            protected override DateTime ExpectedStart => new DateTime(2017, 12, 23);
            protected override DateTime ExpectedEnd => new DateTime(2017, 12, 29);
        }

        public sealed class WhenBeginningOfWeekIsSunday
           : CalendarThisWeekQuickSelectShortcutTests
        {
            protected override BeginningOfWeek BeginningOfWeek => BeginningOfWeek.Sunday;
            protected override DateTimeOffset CurrentTime => new DateTimeOffset(2017, 12, 26, 0, 0, 0, TimeSpan.Zero);
            protected override DateTime ExpectedStart => new DateTime(2017, 12, 24);
            protected override DateTime ExpectedEnd => new DateTime(2017, 12, 30);
        }
    }
}
