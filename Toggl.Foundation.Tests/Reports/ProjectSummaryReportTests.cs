using System;
using System.Linq;
using FluentAssertions;
using FsCheck;
using FsCheck.Xunit;
using Toggl.Foundation.Reports;

namespace Toggl.Foundation.Tests.Reports
{
    public sealed class ProjectSummaryReportTests
    {
        public sealed class TheConstructor
        {
            [Property]
            public void CalculatesThePercentageOfBillable(NonEmptyArray<NonNegativeInt> durations)
            {
                var segments = durations.Get.Select(duration => duration.Get)
                    .Select((index, duration) => getSegmentFromDurationAndIndex(index, duration))
                    .ToArray();
                var totalTrackedSeconds = segments.Select(s => s.TrackedSeconds).Sum();
                var billableSeconds = segments.Select(s => s.BillableSeconds).Sum();
                var expectedBillablePercentage = (100.0f / totalTrackedSeconds) * billableSeconds;

                var report = new ProjectSummaryReport(segments);

                report.BillablePercentage.Should().Be(expectedBillablePercentage);
            }

            [Property]
            public void CalculatesTheTotalAmountOfSeconds(NonEmptyArray<NonNegativeInt> durations)
            {
                var actualDurations = durations.Get.Select(duration => duration.Get);
                var segments = actualDurations
                    .Select((index, duration) => getSegmentFromDurationAndIndex(index, duration))
                    .ToArray();
                var expectedDuration = segments.Select(s => s.TrackedSeconds).Sum();

                var report = new ProjectSummaryReport(segments);

                report.TotalSeconds.Should().Be(expectedDuration);
            }

            private ChartSegment getSegmentFromDurationAndIndex(int index, int trackedSeconds)
                => new ChartSegment("", trackedSeconds, index % 2 == 0 ? trackedSeconds : 0, "#FFFFFF");
        }
    }
}
