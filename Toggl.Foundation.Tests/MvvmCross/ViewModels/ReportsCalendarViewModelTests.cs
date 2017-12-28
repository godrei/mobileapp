﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using FsCheck;
using FsCheck.Xunit;
using NSubstitute;
using Toggl.Foundation.MvvmCross.Parameters;
using Toggl.Foundation.MvvmCross.ViewModels.Calendar;
using Toggl.Foundation.MvvmCross.ViewModels.Calendar.QuickSelectShortcuts;
using Toggl.Foundation.Tests.Generators;
using Toggl.Multivac;
using Toggl.PrimeRadiant.Models;
using Xunit;
using static Toggl.Multivac.Extensions.FunctionalExtensions;

namespace Toggl.Foundation.Tests.MvvmCross.ViewModels
{
    public sealed class ReportsCalendarViewModelTests
    {
        public abstract class ReportsCalendarViewModelTest
            : BaseViewModelTests<ReportsCalendarViewModel>
        {
            protected override ReportsCalendarViewModel CreateViewModel()
                => new ReportsCalendarViewModel(TimeService, DataSource);
        }

        public sealed class TheConstructor : ReportsCalendarViewModelTest
        {
            [Theory, LogIfTooSlow]
            [ClassData(typeof(TwoParameterConstructorTestData))]
            public void ThrowsIfAnyOfTheArgumentsIsNull(bool useTimeService, bool useDataSource)
            {
                var timeService = useTimeService ? TimeService : null;
                var dataSource = useDataSource ? DataSource : null;

                Action tryingToConstructWithEmptyParameters =
                    () => new ReportsCalendarViewModel(timeService, dataSource);

                tryingToConstructWithEmptyParameters
                    .ShouldThrow<ArgumentNullException>();
            }
        }

        public sealed class ThePrepareMethod
            : ReportsCalendarViewModelTest
        {
            [Property]
            public void InitializesCurrentMonthPropertyToCurrentDateTimeOfTimeService(
                DateTimeOffset now)
            {
                TimeService.CurrentDateTime.Returns(now);

                ViewModel.Prepare();

                ViewModel.CurrentMonth.Year.Should().Be(now.Year);
                ViewModel.CurrentMonth.Month.Should().Be(now.Month);
            }
        }

        public sealed class TheInitializeMethod : ReportsCalendarViewModelTest
        {
            [Fact, LogIfTooSlow]
            public async Task InitializesTheMonthsPropertyToLast12Months()
            {
                var now = new DateTimeOffset(2020, 4, 2, 1, 1, 1, TimeSpan.Zero);
                TimeService.CurrentDateTime.Returns(now);
                ViewModel.Prepare();

                await ViewModel.Initialize();

                ViewModel.Months.Should().HaveCount(12);
                var firstDateTime = now.AddMonths(-11);
                var month = new CalendarMonth(
                    firstDateTime.Year, firstDateTime.Month);
                for (int i = 0; i < 12; i++, month = month.Next())
                {
                    ViewModel.Months[i].CalendarMonth.Should().Be(month);
                }
            }

            [Fact, LogIfTooSlow]
            public async Task FillsQuickSelectShortCutlist()
            {
                var expectedShortCuts = new HashSet<Type>
                {
                    typeof(CalendarThisWeekQuickSelectShortcut),
                    typeof(CalendarLastWeekQuickSelectShortcut),
                    typeof(CalendarThisMonthQuickSelectShortcut),
                    typeof(CalendarLastMonthQuickSelectShortcut),
                    typeof(CalendarThisYearQuickSelectShortcut)
                };
                var now = new DateTimeOffset(2020, 4, 2, 1, 1, 1, TimeSpan.Zero);
                TimeService.CurrentDateTime.Returns(now);
                ViewModel.Prepare();

                await ViewModel.Initialize();

                ViewModel.QuickSelectShortcuts.Should().HaveCount(5)
                    .And.OnlyContain(shortcut => expectedShortCuts.Contains(shortcut.GetType()));
            }
        }

        public sealed class TheCurrentMonthProperty : ReportsCalendarViewModelTest
        {
            [Theory]
            [InlineData(2017, 12, 11, 2017, 12)]
            [InlineData(2017, 5, 0, 2016, 6)]
            [InlineData(2017, 5, 11, 2017, 5)]
            [InlineData(2017, 5, 6, 2016, 12)]
            [InlineData(2017, 5, 7, 2017, 1)]
            public void RepresentsTheCurrentPage(
                int currentYear,
                int currentMonth,
                int currentPage,
                int expectedYear,
                int expectedMonth)
            {
                var now = new DateTimeOffset(currentYear, currentMonth, 1, 0, 0, 0, TimeSpan.Zero);
                TimeService.CurrentDateTime.Returns(now);
                ViewModel.Prepare();
                ViewModel.CurrentPage = currentPage;

                ViewModel.CurrentMonth.Year.Should().Be(expectedYear);
                ViewModel.CurrentMonth.Month.Should().Be(expectedMonth);
            }
        }

        public sealed class TheCurrentPageProperty : ReportsCalendarViewModelTest
        {
            [Fact]
            public void IsInitializedTo11()
            {
                ViewModel.CurrentPage.Should().Be(11);
            }
        }

        public sealed class TheRowsInCurrentMonthProperty : ReportsCalendarViewModelTest
        {
            [Theory]
            [InlineData(2017, 12, 11, BeginningOfWeek.Monday, 5)]
            [InlineData(2017, 12, 9, BeginningOfWeek.Monday, 6)]
            [InlineData(2017, 2, 11, BeginningOfWeek.Wednesday, 4)]
            public async Task ReturnsTheRowCountOfCurrentlyShownMonth(
                int currentYear,
                int currentMonth,
                int currentPage,
                BeginningOfWeek beginningOfWeek,
                int expectedRowCount)
            {
                var now = new DateTimeOffset(currentYear, currentMonth, 1, 0, 0, 0, TimeSpan.Zero);
                TimeService.CurrentDateTime.Returns(now);
                var user = Substitute.For<IDatabaseUser>();
                user.BeginningOfWeek.Returns(beginningOfWeek);
                DataSource.User.Current.Returns(Observable.Return(user));
                ViewModel.Prepare();
                await ViewModel.Initialize();
                ViewModel.CurrentPage = currentPage;

                ViewModel.RowsInCurrentMonth.Should().Be(expectedRowCount);
            }
        }

        public sealed class TheSelectedDateRangeObservableProperty : ReportsCalendarViewModelTest
        {
            [Theory, LogIfTooSlow]
            [InlineData(0, 0, 3, 3)]
            [InlineData(5, 23, 9, 0)]
            public void EmitsNewElementWheneverDateRangeIsChangedByTappingTwoCells(
                int startPageIndex,
                int startCellIndex,
                int endPageIndex,
                int endCellIndex)
            {
                var now = new DateTimeOffset(2017, 12, 19, 1, 2, 3, TimeSpan.Zero);
                var observer = Substitute.For<IObserver<DateRangeParameter>>();
                TimeService.CurrentDateTime.Returns(now);
                ViewModel.SelectedDateRangeObservable.Subscribe(observer);
                ViewModel.Prepare();
                ViewModel.Initialize().Wait();
                var startMonth = ViewModel.Months[startPageIndex];
                var firstTappedCellViewModel = startMonth.Days[startCellIndex];
                var endMonth = ViewModel.Months[endPageIndex];
                var secondTappedCellViewModel = endMonth.Days[endCellIndex];
                ViewModel.CalendarDayTappedCommand.Execute(firstTappedCellViewModel);
                ViewModel.CalendarDayTappedCommand.Execute(secondTappedCellViewModel);

                observer.Received().OnNext(Arg.Is<DateRangeParameter>(
                    dateRange => ensureDateRangeIsCorrect(
                        dateRange,
                        firstTappedCellViewModel,
                        secondTappedCellViewModel)));
            }

            private bool ensureDateRangeIsCorrect(
                DateRangeParameter dateRange,
                CalendarDayViewModel expectedStart,
                CalendarDayViewModel expectedEnd)
                    => dateRange.StartDate.Year == expectedStart.CalendarMonth.Year
                    && dateRange.StartDate.Month == expectedStart.CalendarMonth.Month
                    && dateRange.StartDate.Day == expectedStart.Day
                    && dateRange.EndDate.Year == expectedEnd.CalendarMonth.Year
                    && dateRange.EndDate.Month == expectedEnd.CalendarMonth.Month
                    && dateRange.EndDate.Day == expectedEnd.Day;
        }

        public abstract class TheCalendarDayTappedCommand : ReportsCalendarViewModelTest
        {
            public TheCalendarDayTappedCommand()
            {
                var now = new DateTimeOffset(2017, 12, 19, 1, 2, 3, TimeSpan.Zero);
                TimeService.CurrentDateTime.Returns(now);
                ViewModel.Prepare();
                ViewModel.Initialize().Wait();
            }

            protected CalendarDayViewModel FindDayViewModel(int monthIndex, int dayIndex)
                => ViewModel.Months[monthIndex].Days[dayIndex];

            public sealed class AfterTappingOneCell : TheCalendarDayTappedCommand
            {
                [Theory]
                [InlineData(5, 8)]
                public void MarksTheFirstTappedCellAsSelected(
                int monthIndex, int dayIndex)
                {
                    var dayViewModel = FindDayViewModel(monthIndex, dayIndex);

                    ViewModel.CalendarDayTappedCommand.Execute(dayViewModel);

                    dayViewModel.Selected.Should().BeTrue();
                }

                [Theory]
                [InlineData(11, 0)]
                public void MarksTheFirstTappedCellAsStartOfSelection(
                    int monthIndex, int dayIndex)
                {
                    var dayViewModel = FindDayViewModel(monthIndex, dayIndex);

                    ViewModel.CalendarDayTappedCommand.Execute(dayViewModel);

                    dayViewModel.IsStartOfSelectedPeriod.Should().BeTrue();
                }

                [Theory]
                [InlineData(3, 20)]
                public void MarksTheFirstTappedCellAsEndOfSelection(
                    int monthIndex, int dayIndex)
                {
                    var dayViewModel = FindDayViewModel(monthIndex, dayIndex);

                    ViewModel.CalendarDayTappedCommand.Execute(dayViewModel);

                    dayViewModel.IsEndOfSelectedPeriod.Should().BeTrue();
                }

                [Fact]
                public void RemovesAnyPreviousSelection()
                {
                    var preSelectedDays = new(int pageIndex, int dayIndex)[]
                        { (2, 1), (2, 2), (2, 3), (2, 4) };
                    var preSelectedViewModels = preSelectedDays
                        .Select(day => FindDayViewModel(day.pageIndex, day.dayIndex));
                    preSelectedViewModels.ForEach(day => day.Selected = true);
                    var dayToBeSelected = FindDayViewModel(5, 5);

                    ViewModel.CalendarDayTappedCommand.Execute(dayToBeSelected);

                    preSelectedViewModels.ForEach(day => day.Selected.Should().BeFalse());
                }
            }

            public sealed class AfterTappingTwoCells : TheCalendarDayTappedCommand
            {
                [Theory]
                [InlineData(0, 0, 5, 8)]
                public void MarksTheFirstTappedCellAsNotEndOfSelection(
                    int firstMonthIndex,
                    int firstDayindex,
                    int secondMonthIndex,
                    int secondDayIndex)
                {
                    var firstDayViewModel = FindDayViewModel(firstMonthIndex, firstDayindex);
                    var secondDayViewModel = FindDayViewModel(secondMonthIndex, secondDayIndex);

                    ViewModel.CalendarDayTappedCommand.Execute(firstDayViewModel);
                    ViewModel.CalendarDayTappedCommand.Execute(secondDayViewModel);

                    firstDayViewModel.IsEndOfSelectedPeriod.Should().BeFalse();
                }

                [Theory]
                [InlineData(1, 1, 9, 9)]
                public void MarksTheSecondTappedCellAsEndOfSelection(
                    int firstMonthIndex,
                    int firstDayindex,
                    int secondMonthIndex,
                    int secondDayIndex)
                {
                    var firstDayViewModel = FindDayViewModel(firstMonthIndex, firstDayindex);
                    var secondDayViewModel = FindDayViewModel(secondMonthIndex, secondDayIndex);

                    ViewModel.CalendarDayTappedCommand.Execute(firstDayViewModel);
                    ViewModel.CalendarDayTappedCommand.Execute(secondDayViewModel);

                    secondDayViewModel.IsEndOfSelectedPeriod.Should().BeTrue();
                }

                [Theory]
                [InlineData(1, 2, 3, 4)]
                public void MarksTheSecondTappedCellAsNotStartOfSelection(
                    int firstMonthIndex,
                    int firstDayindex,
                    int secondMonthIndex,
                    int secondDayIndex)
                {
                    var firstDayViewModel = FindDayViewModel(firstMonthIndex, firstDayindex);
                    var secondDayViewModel = FindDayViewModel(secondMonthIndex, secondDayIndex);

                    ViewModel.CalendarDayTappedCommand.Execute(firstDayViewModel);
                    ViewModel.CalendarDayTappedCommand.Execute(secondDayViewModel);

                    secondDayViewModel.IsStartOfSelectedPeriod.Should().BeFalse();
                }

                [Theory]
                [InlineData(2, 15, 7, 20)]
                public void MarksTheWholeIntervalAsSelected(
                    int firstMonthIndex,
                    int firstDayindex,
                    int secondMonthIndex,
                    int secondDayIndex)
                {
                    var firstDayViewModel = FindDayViewModel(firstMonthIndex, firstDayindex);
                    var secondDayViewModel = FindDayViewModel(secondMonthIndex, secondDayIndex);

                    ViewModel.CalendarDayTappedCommand.Execute(firstDayViewModel);
                    ViewModel.CalendarDayTappedCommand.Execute(secondDayViewModel);

                    for (int monthIndex = firstMonthIndex; monthIndex <= secondMonthIndex; monthIndex++)
                    {
                        var month = ViewModel.Months[monthIndex];
                        var startIndex = monthIndex == firstMonthIndex
                            ? firstDayindex
                            : 0;
                        var endIndex = monthIndex == secondMonthIndex
                            ? secondDayIndex
                            : month.Days.Count - 1;
                        assertDaysInMonthSelected(month, startIndex, endIndex);
                    }
                }

                private void assertDaysInMonthSelected(
                    CalendarPageViewModel calendarPage, int startindex, int endIndex)
                {
                    for (int i = startindex; i <= endIndex; i++)
                        calendarPage.Days[i].Selected.Should().BeTrue();
                }
            }
        }

        public sealed class TheQuickSelectCommand : ReportsCalendarViewModelTest, IEnumerable<object[]>
        {
            private List<object[]> quickSelectShortcuts;

            public IEnumerator<object[]> GetEnumerator()
                => quickSelectShortcuts.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator()
                => GetEnumerator();

            public TheQuickSelectCommand()
            {
                TimeService.CurrentDateTime.Returns(DateTimeOffset.Now);
                quickSelectShortcuts = new List<object[]>
                {
                    new[] { new CalendarLastWeekQuickSelectShortcut(TimeService, BeginningOfWeek.Monday) },
                    new[] { new CalendarThisWeekQuickSelectShortcut(TimeService, BeginningOfWeek.Monday) },
                    new[] { new CalendarLastMonthQuickSelectShortcut(TimeService) },
                    new[] { new CalendarThisMonthQuickSelectShortcut(TimeService) },
                    new[] { new CalendarThisYearQuickSelectShortcut(TimeService) }
                };
            }

            [Theory, LogIfTooSlow]
            [ClassData(typeof(TheQuickSelectCommand))]
            private async Task SelectsTheRangeOfSelectedShortcut(
                CalendarBaseQuickSelectShortcut quickSelectShortcut)
            {
                var observer = Substitute.For<IObserver<DateRangeParameter>>();
                var expectedRange = quickSelectShortcut.GetDateRange();
                ViewModel.Prepare();
                await ViewModel.Initialize();
                ViewModel.SelectedDateRangeObservable.Subscribe(observer);

                ViewModel.QuickSelectCommand.Execute(quickSelectShortcut);

                observer.Received().OnNext(Arg.Is<DateRangeParameter>(
                    dateRange => dateRange.StartDate == expectedRange.StartDate
                        && dateRange.EndDate == expectedRange.EndDate));
            }
        }
    }
}
