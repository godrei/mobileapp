using System;
using System.Collections.Generic;

namespace Toggl.Multivac.Models.Reports
{
    public interface IProjectsSummary
    {
        DateTimeOffset StartDate { get; }
        DateTimeOffset? EndDate { get; }
        List<IProjectSummary> ProjectsSummaries { get; }
    }
}
