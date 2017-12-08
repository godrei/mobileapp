using System;
using FluentAssertions;
using FsCheck.Xunit;
using NSubstitute;
using Toggl.Foundation.MvvmCross.Parameters;
using Toggl.Foundation.MvvmCross.ViewModels;
using Xunit;

namespace Toggl.Foundation.Tests.MvvmCross.ViewModels
{
    public sealed class ReportsViewModelTests
    {
        public abstract class ReportsViewModelTest
            : BaseViewModelTests<ReportsViewModel>
        {
            protected override ReportsViewModel CreateViewModel()
                => new ReportsViewModel(TimeService);
        }

        public sealed class TheConstructor : ReportsViewModelTest
        {
            [Fact, LogIfTooSlow]
            public void ThrowsIfTheArgumentIsNull()
            {
                Action tryingToConstructWithEmptyParameters =
                    () => new ReportsViewModel(null);

                tryingToConstructWithEmptyParameters
                    .ShouldThrow<ArgumentNullException>();
            }
        }

        public sealed class TheCurrentDateRangeStringProperty : ReportsViewModelTest
        {
            [Fact]
            public void IsInitializedToThisWeek()
            {
                TimeService.CurrentDateTime.Returns(new DateTimeOffset(2017, 10, 10, 10, 10, 10, TimeSpan.Zero));
                ViewModel.Prepare();

                ViewModel.CurrentDateRangeString.Should().Be(Resources.ThisWeek);
            }

            [Theory]
            [InlineData(
                2017, 12, 6,
                2017, 12, 4,
                2017, 12, 10
            )]
            [InlineData(
                2018, 2, 28,
                2018, 2, 26,
                2018, 3, 4
            )]
            [InlineData(
                2016, 4, 20,
                2016, 4, 18,
                2016, 4, 24
            )]
            [InlineData(
                2016, 12, 28,
                2016, 12, 26,
                2017, 1, 1
            )]
            public void ReturnsThisWeekWhenStartAndEndOfCurrentWeekAreSeleted(
                int currentYear, int currentMonth, int currentDay,
                int startYear, int startMonth, int startDay,
                int endYear, int endMonth, int endDay)
            {
                var currentDate = new DateTimeOffset(currentYear, currentMonth, currentDay, 0, 0, 0, TimeSpan.Zero);
                var start = new DateTimeOffset(startYear, startMonth, startDay, 0, 0, 0, TimeSpan.Zero);
                var end = new DateTimeOffset(endYear, endMonth, endDay, 0, 0, 0, TimeSpan.Zero);
                TimeService.CurrentDateTime.Returns(currentDate);
                ViewModel.ChangeDateRangeCommand.Execute(
                    DateRangeParameter.WithStartAndEndDates(start, end));

                ViewModel.CurrentDateRangeString.Should().Be(Resources.ThisWeek);
            }

            [Theory]
            [InlineData(
                 2017, 12, 15,
                 2017, 12, 25,
                 "15 Dec - 25 Dec"
            )]
            [InlineData(
                 2017, 1, 1,
                 2017, 12, 30,
                 "1 Jan - 30 Dec"
            )]
            [InlineData(
                2017, 11, 13,
                2018, 11, 13,
                "13 Nov - 13 Nov"
            )]
            public void ReturnsSelectedDateRangeAsStringIfTheSelectedPeriodIsNotTheCurrentWeek(
                int startYear, int startMonth, int startDay,
                int endYear, int endMonth, int endDay,
                string expectedResult)
            {
                var start = new DateTimeOffset(startYear, startMonth, startDay, 10, 12, 13, TimeSpan.Zero);
                var end = new DateTimeOffset(endYear, endMonth, endDay, 12, 34, 1, TimeSpan.Zero);

                ViewModel.ChangeDateRangeCommand.Execute(
                    DateRangeParameter.WithStartAndEndDates(start, end));

                ViewModel.CurrentDateRangeString.Should().Be(expectedResult);
            }
        }
    }
}
