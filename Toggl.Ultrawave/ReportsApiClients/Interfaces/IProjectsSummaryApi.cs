using System;
using Toggl.Multivac.Models.Reports;

namespace Toggl.Ultrawave.ReportsApiClients
{
    public interface IProjectsSummaryApi
    {
        IObservable<IProjectsSummary> GetByWorkspace(long workspaceId, DateTimeOffset startDate, DateTimeOffset? endDate);
    }
}
