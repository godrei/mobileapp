using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;
using MvvmCross.Core.ViewModels;
using Toggl.Foundation.DataSources;
using Toggl.Multivac;

namespace Toggl.Foundation.MvvmCross.ViewModels
{
    [Preserve(AllMembers = true)]
    public sealed class ReportsCalendarViewModel : MvxViewModel
    {
        private const int monthsToShow = 12;

        private readonly ITimeService timeService;
        private readonly ITogglDataSource dataSource;

        public CalendarMonth CurrentMonth { get; set; }

        public List<CalendarPageViewModel> Months { get; }
            = new List<CalendarPageViewModel>();

        public ReportsCalendarViewModel(
            ITimeService timeService, ITogglDataSource dataSource)
        {
            Ensure.Argument.IsNotNull(timeService, nameof(timeService));
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));

            this.timeService = timeService;
            this.dataSource = dataSource;
        }

        public override void Prepare()
        {
            base.Prepare();

            var now = timeService.CurrentDateTime;
            CurrentMonth = new CalendarMonth(now.Year, now.Month);
        }

        public async override Task Initialize()
        {
            await base.Initialize();

            var beginningOfWeek = (await dataSource.User.Current).BeginningOfWeek;
            var monthIterator = CurrentMonth.AddMonths(-(monthsToShow - 1));
            for (int i = 0; i < 12; i++, monthIterator = monthIterator.Next())
                Months.Add(new CalendarPageViewModel(monthIterator, beginningOfWeek));
        }
    }
}
