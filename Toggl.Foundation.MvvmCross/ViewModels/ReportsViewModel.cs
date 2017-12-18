using System;
using MvvmCross.Core.Navigation;
using MvvmCross.Core.ViewModels;
using Toggl.Foundation.DataSources;
using Toggl.Foundation.MvvmCross.Parameters;
using Toggl.Foundation.MvvmCross.ViewModels.Calendar;
using Toggl.Multivac;

namespace Toggl.Foundation.MvvmCross.ViewModels
{
    [Preserve(AllMembers = true)]
    public sealed class ReportsViewModel : MvxViewModel
    {
        private const string dateFormat = "d MMM";

        private DateTimeOffset startDate;
        private DateTimeOffset endDate;

        private readonly ITimeService timeService;
        private readonly IMvxNavigationService navigationService;
        private readonly ReportsCalendarViewModel calendarViewModel;

        public bool HasData { get; }

        public string CurrentDateRangeString { get; private set; }

        public bool CurrentWeekSelected => false;

        public bool CalendarVisible { get; private set; }

        public IMvxCommand<DateRangeParameter> ChangeDateRangeCommand { get; }
        public IMvxCommand ShowCalendarCommand { get; }

        public ReportsViewModel(
            ITimeService timeService,
            IMvxNavigationService navigationService,
            ITogglDataSource dataSource)
        {
            Ensure.Argument.IsNotNull(timeService, nameof(timeService));
            Ensure.Argument.IsNotNull(navigationService, nameof(navigationService));
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));

            this.timeService = timeService;
            this.navigationService = navigationService;

            calendarViewModel = new ReportsCalendarViewModel(timeService, dataSource);

            ChangeDateRangeCommand = new MvxCommand<DateRangeParameter>(changeDateRange);
            ShowCalendarCommand = new MvxCommand(showCalendar);
        }

        public override void Prepare()
        {
            var currentDate = timeService.CurrentDateTime.Date;
            startDate = currentDate.AddDays(
                1 -
                (int)currentDate.DayOfWeek);
            endDate = startDate.AddDays(6);
            updateCurrentDateRangeString();

            calendarViewModel.SelectedDateRangeObservable.Subscribe(
                newDateRange => ChangeDateRangeCommand.Execute(newDateRange)
            );
        }

        private void changeDateRange(DateRangeParameter dateRange)
        {
            startDate = dateRange.StartDate;
            endDate = dateRange.EndDate;
            updateCurrentDateRangeString();
        }

        private void updateCurrentDateRangeString()
            => CurrentDateRangeString = CurrentWeekSelected
                ? Resources.ThisWeek
                : $"{startDate.ToString(dateFormat)} - {endDate.ToString(dateFormat)}";

        public override void ViewAppeared()
        {
            base.ViewAppeared();
            navigationService.Navigate(calendarViewModel);
        }

        private void showCalendar() => CalendarVisible = !CalendarVisible;
    }
}
