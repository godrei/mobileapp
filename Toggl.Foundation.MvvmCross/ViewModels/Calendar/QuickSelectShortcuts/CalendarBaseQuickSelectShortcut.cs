using System.ComponentModel;
using Toggl.Foundation.MvvmCross.Parameters;
using Toggl.Multivac;

namespace Toggl.Foundation.MvvmCross.ViewModels.Calendar.QuickSelectShortcuts
{
    public abstract class CalendarBaseQuickSelectShortcut : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected ITimeService TimeService { get; private set; }

        public string Title { get; }

        public bool Selected { get; private set; }

        protected CalendarBaseQuickSelectShortcut(
            ITimeService timeService, string title)
        {
            Ensure.Argument.IsNotNull(timeService, nameof(timeService));

            TimeService = timeService;

            Title = title;
        }

        public void OnDateRangeChanged(DateRangeParameter dateRange)
        {
            var thisActionDateRange = GetDateRange();

            Selected = dateRange.StartDate.Date == thisActionDateRange.StartDate.Date
                    && dateRange.EndDate.Date == thisActionDateRange.EndDate.Date;
        }

        public abstract DateRangeParameter GetDateRange();
    }
}
