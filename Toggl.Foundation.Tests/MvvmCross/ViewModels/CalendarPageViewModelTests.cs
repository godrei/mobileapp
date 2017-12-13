using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using FsCheck;
using FsCheck.Xunit;
using Toggl.Foundation.MvvmCross.ViewModels;
using Toggl.Multivac;
using Xunit;

namespace Toggl.Foundation.Tests.MvvmCross.ViewModels
{
    public sealed class CalendarPageViewModelTests
    {
        private sealed class CalendarPageTestData : IEnumerable<object[]>
        {
            private List<object[]> data = new List<object[]>
            {
                new object[] { 2017, 12, BeginningOfWeek.Monday, 4, 0 },
                new object[] { 2017, 12, BeginningOfWeek.Sunday, 5, 6 },
                new object[] { 2017, 7, BeginningOfWeek.Saturday, 0, 4 },
                new object[] { 2017, 11, BeginningOfWeek.Thursday, 6, 6 },
                new object[] { 2017, 2, BeginningOfWeek.Wednesday, 0, 0 }
            };

            public IEnumerator<object[]> GetEnumerator()
                => data.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator()
                => GetEnumerator();
        }

        public sealed class TheDaysProperty
        {
            private CalendarMonth calendarMonth;
            private CalendarPageViewModel viewModel;

            private void prepare(
                int year, int month, BeginningOfWeek beginningOfWeek)
            {
                calendarMonth = new CalendarMonth(year, month);
                viewModel = new CalendarPageViewModel(calendarMonth, beginningOfWeek);
            }

            [Theory]
            [ClassData(typeof(CalendarPageTestData))]
            public void ContainsFewDaysFromPreviousMonthAtTheBeginning(
                int year,
                int month,
                BeginningOfWeek beginningOfWeek,
                int expectedDayCount,
                int _)
            {
                prepare(year, month, beginningOfWeek);

                if (expectedDayCount == 0)
                    viewModel.Days.First().IsInCurrentMonth.Should().BeTrue();
                else
                    viewModel
                        .Days
                        .Take(expectedDayCount)
                        .Should()
                        .OnlyContain(day => !day.IsInCurrentMonth);
            }

            [Theory]
            [ClassData(typeof(CalendarPageTestData))]
            public void ConainsAllDaysFromCurrentMonthInTheMiddle(
                int year,
                int month,
                BeginningOfWeek beginningOfWeek,
                int daysFromLastMonth,
                int _)
            {
                prepare(year, month, beginningOfWeek);

                viewModel
                    .Days
                    .Skip(daysFromLastMonth)
                    .Take(calendarMonth.DaysInMonth)
                    .Should()
                    .OnlyContain(day => day.IsInCurrentMonth);
            }

            [Theory]
            [ClassData(typeof(CalendarPageTestData))]
            public void ContainsFewDaysFromNextMonthAtTheEnd(
                int year,
                int month,
                BeginningOfWeek beginningOfWeek,
                int daysFromLastMonth,
                int expectedDayCount)
            {
                prepare(year, month, beginningOfWeek);
                var daysFromNextMonth = viewModel
                    .Days
                    .Skip(daysFromLastMonth)
                    .Skip(calendarMonth.DaysInMonth);

                if (expectedDayCount == 0)
                    daysFromNextMonth.Should().BeEmpty();
                else
                    daysFromNextMonth
                        .Should()
                        .OnlyContain(day => !day.IsInCurrentMonth);
            }

            [Property]
            public void AlwaysIsAMultipleOf7(
                NonNegativeInt year,
                NonNegativeInt month,
                BeginningOfWeek beginingOfWeek)
            {
                prepare(year.Get % 9999 + 2, month.Get % 12 + 1, beginingOfWeek);

                (viewModel.Days.Count % 7).Should().Be(0);
            }
        }
    }
}
