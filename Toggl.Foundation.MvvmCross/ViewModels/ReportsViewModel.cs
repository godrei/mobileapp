using System;
using MvvmCross.Core.ViewModels;
using Toggl.Foundation.MvvmCross.Parameters;
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

        public bool HasData { get; }

        public string CurrentDateRangeString { get; private set; }

        public bool IsCurrentWeek
        {
            get
            {
                var currentDate = timeService.CurrentDateTime.Date;
                var startOfWeek = currentDate.AddDays(
                    1 -
                    (int)currentDate.DayOfWeek);
                var endOfWeek = startOfWeek.AddDays(6);

                return startDate.Date == startOfWeek
                       && endDate.Date == endOfWeek;
            }
        }

        public IMvxCommand<DateRangeParameter> ChangeDateRangeCommand { get; }

        public ReportsViewModel(ITimeService timeService)
        {
            Ensure.Argument.IsNotNull(timeService, nameof(timeService));

            this.timeService = timeService;

            ChangeDateRangeCommand = new MvxCommand<DateRangeParameter>(changeDateRange);
        }

        public override void Prepare()
        {
            var currentDate = timeService.CurrentDateTime.Date;
            startDate = currentDate.AddDays(
                1 -
                (int)currentDate.DayOfWeek);
            endDate = startDate.AddDays(6);
            updateCurrentDateRangeString();
        }

        private void changeDateRange(DateRangeParameter dateRange)
        {
            startDate = dateRange.StartDate;
            endDate = dateRange.EndDate;
            updateCurrentDateRangeString();
        }

        private void updateCurrentDateRangeString()
            => CurrentDateRangeString = IsCurrentWeek
                ? Resources.ThisWeek
                : $"{startDate.ToString(dateFormat)} - {endDate.ToString(dateFormat)}";
    }
}
