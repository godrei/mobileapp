using System;
namespace Toggl.Foundation.MvvmCross.Parameters
{
    public sealed class DateRangeParameter
    {
        public DateTimeOffset StartDate { get; set; }

        public DateTimeOffset EndDate { get; set; }

        public static DateRangeParameter WithStartAndEndDates(
            DateTimeOffset startDate, DateTimeOffset endDate)
            => new DateRangeParameter { StartDate = startDate, EndDate = endDate };
    }
}
