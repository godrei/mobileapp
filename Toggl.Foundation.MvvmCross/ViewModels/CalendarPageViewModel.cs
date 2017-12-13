using System.Collections.Generic;
using Toggl.Multivac;

namespace Toggl.Foundation.MvvmCross.ViewModels
{
    public sealed class CalendarPageViewModel
    {
        private readonly BeginningOfWeek beginningOfWeek;

        public List<CalendarDayViewModel> Days { get; }
            = new List<CalendarDayViewModel>();

        public CalendarMonth CalendarMonth { get; }

        public CalendarPageViewModel(
            CalendarMonth calendarMonth, BeginningOfWeek beginningOfWeek)
        {
            CalendarMonth = calendarMonth;

            this.beginningOfWeek = beginningOfWeek;

            addDaysFromPreviousMonth();
            addDaysFromCurrentMonth();
            addDaysFromNextMonth();
        }

        private void addDaysFromPreviousMonth()
        {
            var firstDayOfMonth = CalendarMonth.DayOfWeek(1);

            if (firstDayOfMonth == beginningOfWeek.ToDayOfWeekEnum()) return;

            var previousMonth = CalendarMonth.Previous();
            var daysInPreviousMonth = previousMonth.DaysInMonth;
            var daysToAdd = ((int)firstDayOfMonth - (int)beginningOfWeek.ToDayOfWeekEnum() + 7) % 7;

            for (int i = daysToAdd - 1; i >= 0; i--)
                Days.Add(new CalendarDayViewModel(daysInPreviousMonth - i, false));
        }

        private void addDaysFromCurrentMonth()
        {
            var daysInMonth = CalendarMonth.DaysInMonth;
            for (int i = 0; i < daysInMonth; i++)
                Days.Add(new CalendarDayViewModel(i + 1, true));
        }

        private void addDaysFromNextMonth()
        {
            var lastDayOfWeekInTargetMonth = (int)CalendarMonth
                .DayOfWeek(CalendarMonth.DaysInMonth);

            var lastDayOfWeek = ((int)beginningOfWeek + 6) % 7;
            var daysToAdd = (lastDayOfWeek - lastDayOfWeekInTargetMonth + 7) % 7;

            for (int i = 0; i < daysToAdd; i++)
                Days.Add(new CalendarDayViewModel(i + 1, false));
        }
    }
}
