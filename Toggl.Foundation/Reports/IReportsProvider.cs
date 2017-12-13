using System;
namespace Toggl.Foundation.Reports
{
    public interface IReportsProvider
    {
        IObservable<ProjectSummaryReport> GetProjectSummary(
            long workspaceId, DateTimeOffset startDate, DateTimeOffset? endDate);
    }
}
