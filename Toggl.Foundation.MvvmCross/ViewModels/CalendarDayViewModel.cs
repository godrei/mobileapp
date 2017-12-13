using Toggl.Multivac;

namespace Toggl.Foundation.MvvmCross.ViewModels
{
    [Preserve(AllMembers = true)]
    public sealed class CalendarDayViewModel
    {
        public int Day { get; }

        public bool IsInCurrentMonth { get; set; }

        public CalendarDayViewModel(int day, bool isInCurrentMonth)
        {
            Day = day;
            IsInCurrentMonth = isInCurrentMonth;
        }
    }
}
