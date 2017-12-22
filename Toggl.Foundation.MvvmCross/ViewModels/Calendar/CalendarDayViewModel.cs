using System;
using System.ComponentModel;
using Toggl.Multivac;

namespace Toggl.Foundation.MvvmCross.ViewModels.Calendar
{
    [Preserve(AllMembers = true)]
    public sealed class CalendarDayViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public int Day { get; }

        public CalendarMonth CalendarMonth { get; }

        public bool IsInCurrentMonth { get; set; }

        public bool Selected { get; set; }

        public bool IsToday { get; set; }
        
        public bool IsStartOfSelectedPeriod { get; set; }

        public bool IsEndOfSelectedPeriod { get; set; }

        public CalendarDayViewModel(int day, CalendarMonth month, bool isInCurrentMonth, bool isToday)
        {
            Day = day;
            CalendarMonth = month;
            IsInCurrentMonth = isInCurrentMonth;
            IsToday = isToday;
        }

        public DateTimeOffset ToDateTimeOffset()
            => new DateTimeOffset(
                CalendarMonth.Year,
                CalendarMonth.Month,
                Day,
                0,
                0,
                0,
                TimeSpan.Zero);
    }
}
