using System;
using System.Threading.Tasks;
using FluentAssertions;
using FsCheck.Xunit;
using NSubstitute;
using Toggl.Foundation.MvvmCross.ViewModels.Calendar;
using Toggl.Foundation.Tests.Generators;
using Toggl.Multivac;
using Xunit;

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
        }
    }
}
