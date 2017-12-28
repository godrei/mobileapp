using System;
using Toggl.Foundation.MvvmCross.ViewModels.Calendar.QuickSelectShortcuts;
using Toggl.Multivac;

namespace Toggl.Foundation.Tests.MvvmCross.ViewModels.CalendarQuickSelectShortcuts
{
    public abstract class CalendarLastWeekQuickSelectShortcutTests
        : BaseCalendarQuickSelectShortcutTests<CalendarLastWeekQuickSelectShortcut>
    {
        protected abstract BeginningOfWeek BeginningOfWeek { get; }

        protected sealed override CalendarLastWeekQuickSelectShortcut CreateQuickSelectShortCut()
            => new CalendarLastWeekQuickSelectShortcut(TimeService, BeginningOfWeek);

        protected sealed override CalendarLastWeekQuickSelectShortcut TryToCreateQuickSelectShortCutWithNull()
            => new CalendarLastWeekQuickSelectShortcut(null, BeginningOfWeek);

        public sealed class WhenBeginningOfWeekIsMonday
            : CalendarLastWeekQuickSelectShortcutTests
        {
            protected override BeginningOfWeek BeginningOfWeek => BeginningOfWeek.Monday;
            protected override DateTimeOffset CurrentTime => new DateTimeOffset(2017, 12, 26, 1, 1, 1, TimeSpan.Zero);
            protected override DateTime ExpectedStart => new DateTime(2017, 12, 18);
            protected override DateTime ExpectedEnd => new DateTime(2017, 12, 24);
        }

        public sealed class WhenBeginningOfWeekIsTuesday
            : CalendarLastWeekQuickSelectShortcutTests
        {
            protected override BeginningOfWeek BeginningOfWeek => BeginningOfWeek.Tuesday;
            protected override DateTimeOffset CurrentTime => new DateTimeOffset(2017, 12, 26, 0, 0, 0, TimeSpan.Zero);
            protected override DateTime ExpectedStart => new DateTime(2017, 12, 19);
            protected override DateTime ExpectedEnd => new DateTime(2017, 12, 25);
        }

        public sealed class WhenBeginningOfWeekIsWednesday
            : CalendarLastWeekQuickSelectShortcutTests
        {
            protected override BeginningOfWeek BeginningOfWeek => BeginningOfWeek.Wednesday;
            protected override DateTimeOffset CurrentTime => new DateTimeOffset(2017, 12, 26, 0, 0, 0, TimeSpan.Zero);
            protected override DateTime ExpectedStart => new DateTime(2017, 12, 13);
            protected override DateTime ExpectedEnd => new DateTime(2017, 12, 19);
        }

        public sealed class WhenBeginningOfWeekIsThursday
            : CalendarLastWeekQuickSelectShortcutTests
        {
            protected override BeginningOfWeek BeginningOfWeek => BeginningOfWeek.Thursday;
            protected override DateTimeOffset CurrentTime => new DateTimeOffset(2017, 12, 26, 0, 0, 0, TimeSpan.Zero);
            protected override DateTime ExpectedStart => new DateTime(2017, 12, 14);
            protected override DateTime ExpectedEnd => new DateTime(2017, 12, 20);
        }

        public sealed class WhenBeginningOfWeekIsFriday
            : CalendarLastWeekQuickSelectShortcutTests
        {
            protected override BeginningOfWeek BeginningOfWeek => BeginningOfWeek.Friday;
            protected override DateTimeOffset CurrentTime => new DateTimeOffset(2017, 12, 26, 0, 0, 0, TimeSpan.Zero);
            protected override DateTime ExpectedStart => new DateTime(2017, 12, 15);
            protected override DateTime ExpectedEnd => new DateTime(2017, 12, 21);
        }

        public sealed class WhenBeginningOfWeekIsSaturday
            : CalendarLastWeekQuickSelectShortcutTests
        {
            protected override BeginningOfWeek BeginningOfWeek => BeginningOfWeek.Saturday;
            protected override DateTimeOffset CurrentTime => new DateTimeOffset(2017, 12, 26, 0, 0, 0, TimeSpan.Zero);
            protected override DateTime ExpectedStart => new DateTime(2017, 12, 16);
            protected override DateTime ExpectedEnd => new DateTime(2017, 12, 22);
        }

        public sealed class WhenBeginningOfWeekIsSunday
            : CalendarLastWeekQuickSelectShortcutTests
        {
            protected override BeginningOfWeek BeginningOfWeek => BeginningOfWeek.Sunday;
            protected override DateTimeOffset CurrentTime => new DateTimeOffset(2017, 12, 26, 0, 0, 0, TimeSpan.Zero);
            protected override DateTime ExpectedStart => new DateTime(2017, 12, 17);
            protected override DateTime ExpectedEnd => new DateTime(2017, 12, 23);
        }
    }
}
