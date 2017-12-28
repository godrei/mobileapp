using System;
using FluentAssertions;
using NSubstitute;
using Toggl.Foundation.MvvmCross.ViewModels.Calendar.QuickSelectShortcuts;
using Xunit;

namespace Toggl.Foundation.Tests.MvvmCross.ViewModels.CalendarQuickSelectShortcuts
{
    public abstract class BaseCalendarQuickSelectShortcutTests<T>
        where T : CalendarBaseQuickSelectShortcut
    {
        protected ITimeService TimeService { get; } = Substitute.For<ITimeService>();

        protected BaseCalendarQuickSelectShortcutTests()
        {
            TimeService.CurrentDateTime.Returns(CurrentTime);
        }

        protected abstract T CreateQuickSelectShortCut();
        protected abstract T TryToCreateQuickSelectShortCutWithNull();

        protected abstract DateTimeOffset CurrentTime { get; }
        protected abstract DateTime ExpectedStart { get; }
        protected abstract DateTime ExpectedEnd { get; }

        [Fact]
        public void SetsSelectedToTrueWhenReceivesOnDateRangeChangedWithOwnDateRange()
        {
            var quickSelectShortCut = CreateQuickSelectShortCut();
            var dateRange = quickSelectShortCut.GetDateRange();

            quickSelectShortCut.OnDateRangeChanged(dateRange);

            quickSelectShortCut.Selected.Should().BeTrue();
        }

        [Fact]
        public void TheGetDateRangeReturnsExpectedDateRange()
        {
            var dateRange = CreateQuickSelectShortCut().GetDateRange();

            dateRange.StartDate.Date.Should().Be(ExpectedStart);
            dateRange.EndDate.Date.Should().Be(ExpectedEnd);
        }

        [Fact]
        public void ConstructorThrowsWhenTryingToConstructWithNull()
        {
            Action tryingToConstructWithNull =
                () => TryToCreateQuickSelectShortCutWithNull();

            tryingToConstructWithNull.ShouldThrow<ArgumentNullException>();
        }
    }
}
