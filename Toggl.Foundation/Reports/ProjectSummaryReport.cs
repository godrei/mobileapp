using System.Linq;

namespace Toggl.Foundation.Reports
{
    public sealed class ProjectSummaryReport
    {
        public float TotalSeconds { get; }

        public float BillablePercentage { get; }

        public ChartSegment[] Segments { get; }

        public ProjectSummaryReport(ChartSegment[] segments)
        {
            var totalSeconds = segments.Select(x => x.TrackedSeconds).Sum();
            var billableSeconds = segments.Select(x => x.BillableSeconds).Sum();

            Segments = segments;
            TotalSeconds = totalSeconds;
            BillablePercentage = (100.0f / totalSeconds) * billableSeconds;
        }
    }
}
