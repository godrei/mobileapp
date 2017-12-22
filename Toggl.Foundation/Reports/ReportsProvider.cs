using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Toggl.Foundation.Helper;
using Toggl.Foundation.Models;
using Toggl.Multivac;
using Toggl.Multivac.Extensions;
using Toggl.Multivac.Models;
using Toggl.Multivac.Models.Reports;
using Toggl.PrimeRadiant;
using Toggl.PrimeRadiant.Models;
using Toggl.Ultrawave;
using Toggl.Ultrawave.ApiClients;

namespace Toggl.Foundation.Reports
{
    public sealed class ReportsProvider : IReportsProvider
    {
        private readonly object listLock = new object();

        private readonly IProjectsApi projectsApi;
        private readonly IProjectsSummaryApi projectSummaryApi;
        private readonly IRepository<IDatabaseProject> projectsRepository;

        private readonly IList<IProject> memoryCache = new List<IProject>();

        public ReportsProvider(ITogglApi api, ITogglDatabase database)
        {
            Ensure.Argument.IsNotNull(api, nameof(api));
            Ensure.Argument.IsNotNull(database, nameof(database));

            projectsApi = api.Projects;
            projectsRepository = database.Projects;
            projectSummaryApi = api.ProjectsSummary;
        }

        public IObservable<ProjectSummaryReport> GetProjectSummary(
            long workspaceId, DateTimeOffset startDate, DateTimeOffset? endDate)
            => projectSummaryApi
                .GetByWorkspace(workspaceId, startDate, endDate)
                .SelectMany(response => summaryReportFromResponse(response, workspaceId));

        private IObservable<ProjectSummaryReport> summaryReportFromResponse(IProjectsSummary response, long workspaceId)
            => response.ProjectsSummaries
                    .Select(s => s.ProjectId)
                    .SelectNonNulls()
                    .Apply(ids => searchProjects(workspaceId, ids.ToArray()))
                    .Select(projectsInReport => 
                    {
                        var totalSeconds = response.ProjectsSummaries.Select(s => s.TrackedSeconds).Sum();
                        return response.ProjectsSummaries
                            .Select(summary => chartFromSummary(summary, projectsInReport, totalSeconds));
            
                    })
                    .Select(segments => 
                        new ProjectSummaryReport(segments.OrderByDescending(c => c.Percentage).ToArray()));

        private static ChartSegment chartFromSummary(IProjectSummary summary, 
                                                     IList<IProject> projectsInReport,
                                                     long totalSeconds)
        {
            var percentage = totalSeconds == 0 ? 0 : (summary.TrackedSeconds / (float)totalSeconds) * 100;
            var billableSeconds = summary.BillableSeconds ?? 0;
            var project = projectsInReport.FirstOrDefault(p => p.Id == summary.ProjectId);
            return project == null
                ? new ChartSegment(Resources.NoProject, percentage,  summary.TrackedSeconds, billableSeconds, Color.NoProject)
                : new ChartSegment(project.Name, percentage, summary.TrackedSeconds, billableSeconds, project.Color);
        }

        private IObservable<IList<IProject>> searchProjects(long workspaceId, long[] projectIds) =>
            Observable.DeferAsync(async cancellationToken =>
            {
                if (projectIds.Length == 0)
                    return Observable.Return(new List<IProject>());

                var projectsInDatabase = await projectIds
                    .Select(tryGetProjectFromDatabase)
                    .Aggregate(Observable.Merge)
                    .ToList();

                var notInDatabaseIds = projectsInDatabase.SelectAllLeft().ToArray();
                var databaseProjectsObservable =
                    Observable.Return(projectsInDatabase.SelectAllRight().ToList());

                return notInDatabaseIds.Length == 0
                    ? databaseProjectsObservable
                    : databaseProjectsObservable
                        .Merge(searchMemoryAndApi(workspaceId, notInDatabaseIds))
                        .SelectMany(list => list)
                        .ToList();
            });

        private IObservable<Either<long, IProject>> tryGetProjectFromDatabase(long id)
            => projectsRepository.GetById(id)
                .Select(Project.From)
                .Select(Either<long, IProject>.WithRight)
                .Catch(Observable.Return(Either<long, IProject>.WithLeft(id)));

        private IObservable<IList<IProject>> searchMemoryAndApi(long workspaceId, long[] projectIds) =>
            Observable.Defer<IList<IProject>>(() =>
            {
                lock (listLock)
                {
                    var projectsInMemory = projectIds.Select(tryGetProjectFromMemoryCache);

                    var notInMemoryIds = projectsInMemory.SelectAllLeft().ToArray();
                    var memoryProjectsObservable =
                        Observable.Return(projectsInMemory.SelectAllRight().ToList());

                    return notInMemoryIds.Length == 0
                        ? memoryProjectsObservable
                        : memoryProjectsObservable
                            .Merge(searchApi(workspaceId, notInMemoryIds))
                            .SelectMany(list => list)
                            .ToList();
                }
            });

        private Either<long, IProject> tryGetProjectFromMemoryCache(long id)
        {
            var project = memoryCache.FirstOrDefault(p => p.Id == id);
            return project == null
                ? Either<long, IProject>.WithLeft(id)
                : Either<long, IProject>.WithRight(project);
        }

        private IObservable<List<IProject>> searchApi(long workspaceId, long[] projectIds)
            => projectsApi.Search(workspaceId, projectIds)
                .Do(persistInMemoryCache);

        private void persistInMemoryCache(List<IProject> apiProjects)
        {
            lock (listLock)
            {
                apiProjects.ForEach(memoryCache.Add);
            }
        }
    }
}
