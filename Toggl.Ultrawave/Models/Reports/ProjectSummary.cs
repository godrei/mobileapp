using Toggl.Multivac.Models.Reports;

namespace Toggl.Ultrawave.Models.Reports
{
    internal sealed class ProjectSummary : IProjectSummary
    {
        public long UserId { get; set; }

        public long? ProjectId { get; set; }

        public long TrackedSeconds { get; set; }

        public long? BillableSeconds { get; set; }
    }
}
