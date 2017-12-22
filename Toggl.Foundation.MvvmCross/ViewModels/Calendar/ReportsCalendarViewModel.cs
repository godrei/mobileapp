using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using MvvmCross.Core.ViewModels;
using PropertyChanged;
using Toggl.Foundation.DataSources;
using Toggl.Foundation.MvvmCross.Parameters;
using Toggl.Foundation.MvvmCross.ViewModels.Calendar.QuickSelectShortcuts;
using Toggl.Multivac;
using Toggl.Multivac.Extensions;

namespace Toggl.Foundation.MvvmCross.ViewModels.Calendar
{
    [Preserve(AllMembers = true)]
    public sealed class ReportsCalendarViewModel : MvxViewModel
    {
        private const int monthsToShow = 12;

        //Fields
        private readonly ITimeService timeService;
        private readonly ITogglDataSource dataSource;
        private readonly ISubject<DateRangeParameter> selectedDateRangeSubject = new Subject<DateRangeParameter>();

        private CalendarMonth initialMonth;
        private BeginningOfWeek beginningOfWeek;
        private CalendarDayViewModel startOfSelection;

        //Properties
        [DependsOn(nameof(CurrentPage))]
        public CalendarMonth CurrentMonth => convertPageIndexTocalendarMonth(CurrentPage);

        public int CurrentPage { get; set; } = monthsToShow - 1;

        [DependsOn(nameof(Months), nameof(CurrentPage))]
        public int RowsInCurrentMonth => Months[CurrentPage].RowCount;

        public List<CalendarPageViewModel> Months { get; } = new List<CalendarPageViewModel>();

        public IObservable<DateRangeParameter> SelectedDateRangeObservable
            => selectedDateRangeSubject.AsObservable();

        public List<CalendarBaseQuickSelectShortcut> QuickSelectShortcuts { get; private set; }

        public IMvxCommand<CalendarDayViewModel> CalendarDayTappedCommand { get; }

        public IMvxCommand<CalendarBaseQuickSelectShortcut> QuickSelectCommand { get; }

        public ReportsCalendarViewModel(
            ITimeService timeService, ITogglDataSource dataSource)
        {
            Ensure.Argument.IsNotNull(timeService, nameof(timeService));
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));

            this.timeService = timeService;
            this.dataSource = dataSource;

            CalendarDayTappedCommand = new MvxCommand<CalendarDayViewModel>(calendarDayTapped);
            QuickSelectCommand = new MvxCommand<CalendarBaseQuickSelectShortcut>(quickSelect);
        }

        private void calendarDayTapped(CalendarDayViewModel tappedDay)
        {
            if (startOfSelection == null)
            {
                clearAllHighlights();
                startOfSelection = tappedDay;
                startOfSelection.Selected
                    = startOfSelection.IsStartOfSelectedPeriod
                    = startOfSelection.IsEndOfSelectedPeriod
                    = true;
                return;
            }

            var startDate = startOfSelection.ToDateTimeOffset();
            var endDate = tappedDay.ToDateTimeOffset();

            startOfSelection.IsEndOfSelectedPeriod = false;

            var dateRange = DateRangeParameter.WithDates(startDate, endDate);
            changeDateRange(dateRange);
            startOfSelection = null;
        }

        public override void Prepare()
        {
            base.Prepare();

            var now = timeService.CurrentDateTime;
            initialMonth = new CalendarMonth(now.Year, now.Month).AddMonths(-(monthsToShow - 1));
        }

        public async override Task Initialize()
        {
            await base.Initialize();

            beginningOfWeek = (await dataSource.User.Current).BeginningOfWeek;
            fillMonthArray();
            RaisePropertyChanged(nameof(CurrentMonth));

            QuickSelectShortcuts = createQuickSelectShortcuts();

            QuickSelectShortcuts.ForEach(quickSelectShortcut =>
                SelectedDateRangeObservable.Subscribe(
                    quickSelectShortcut.OnDateRangeChanged));
        }

        private void fillMonthArray()
        {
            var monthIterator = initialMonth;
            for (int i = 0; i < 12; i++, monthIterator = monthIterator.Next())
                Months.Add(new CalendarPageViewModel(monthIterator, beginningOfWeek, timeService.CurrentDateTime));
        }

        private List<CalendarBaseQuickSelectShortcut> createQuickSelectShortcuts()
            => new List<CalendarBaseQuickSelectShortcut>
            {
                new CalendarThisWeekQuickSelectShortcut(timeService, beginningOfWeek),
                new CalendarLastWeekQuickSelectShortcut(timeService, beginningOfWeek),
                new CalendarThisMonthQuickSelectShortcut(timeService),
                new CalendarLastMonthQuickSelectShortcut(timeService),
                new CalendarThisYearQuickSelectShortcut(timeService)
            };

        private CalendarMonth convertPageIndexTocalendarMonth(int pageIndex)
            => initialMonth.AddMonths(pageIndex);

        private int convertCalendarMonthToPageIndex(CalendarMonth calendarMonth)
        {
            var initialTotalMonths = initialMonth.Year * 12 + initialMonth.Month;
            var totalMonths = calendarMonth.Year * 12 + calendarMonth.Month;
            return totalMonths - initialTotalMonths;
        }

        private void changeDateRange(DateRangeParameter newDateRange)
        {
            startOfSelection = null;

            highlightDateRange(newDateRange);

            selectedDateRangeSubject.OnNext(newDateRange);
        }

        private void quickSelect(CalendarBaseQuickSelectShortcut quickSelectShortCut)
            => changeDateRange(quickSelectShortCut.GetDateRange());

        private void highlightDateRange(DateRangeParameter dateRange)
        {
            clearAllHighlights();

            //Mark start
            var startDayViewModels = findDayViewModelsForDate(dateRange.StartDate);
            startDayViewModels.ForEach(d => d.IsStartOfSelectedPeriod = true);

            //Mark end
            var endDayViewModels = findDayViewModelsForDate(dateRange.EndDate);
            endDayViewModels.ForEach(d => d.IsEndOfSelectedPeriod = true);

            //Mark dates as selected
            for (DateTime i = dateRange.StartDate.Date; i <= dateRange.EndDate.Date; i = i.AddDays(1))
                findDayViewModelsForDate(i).ForEach(day => day.Selected = true);
        }

        private void clearAllHighlights()
        {
            foreach (var month in Months)
            {
                foreach (var day in month.Days)
                {
                    day.Selected
                        = day.IsStartOfSelectedPeriod
                        = day.IsEndOfSelectedPeriod
                        = false;
                }
            }
        }

        private List<CalendarDayViewModel> findDayViewModelsForDate(DateTimeOffset date)
        {
            List<CalendarDayViewModel> results = new List<CalendarDayViewModel>();
            var calendarMonth = new CalendarMonth(date.Year, date.Month);
            var page = convertCalendarMonthToPageIndex(calendarMonth);

            if (page >= 0 && page < monthsToShow)
            {
                results
                    .Add(Months[page]
                    .Days
                    .Single(dayViewModel
                            => dayViewModel.CalendarMonth == calendarMonth
                            && dayViewModel.Day == date.Day));
            }
            
            if (date.Day < 7 && page > 0)
            {
                var otherResult = Months[page - 1]
                    .Days
                    .SingleOrDefault(dayViewModel
                        => !dayViewModel.IsInCurrentMonth
                        && dayViewModel.Day == date.Day);
                
                if (otherResult != null)
                    results.Add(otherResult);
            }

            if (date.Day > calendarMonth.DaysInMonth - 7 && page < monthsToShow - 1)
            {
                var otherResult = Months[page + 1]
                    .Days
                    .SingleOrDefault(dayViewModel
                        => !dayViewModel.IsInCurrentMonth
                        && dayViewModel.Day == date.Day);

                if (otherResult != null)
                    results.Add(otherResult);
            }

            return results;
        }
    }
}
