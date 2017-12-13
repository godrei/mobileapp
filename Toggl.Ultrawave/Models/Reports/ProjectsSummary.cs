using System;
using System.Collections.Generic;
using Toggl.Multivac;
using Toggl.Multivac.Models.Reports;

namespace Toggl.Ultrawave.Models.Reports
{
    [Preserve(AllMembers = true)]
    internal sealed class ProjectsSummary : IProjectsSummary
    {
        public DateTimeOffset StartDate { get; set; }

        public DateTimeOffset? EndDate { get; set; }

        public List<IProjectSummary> ProjectsSummaries { get; set; }
    }
}
