using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using Toggl.Foundation.DTOs;
using Toggl.Foundation.Models;
using Toggl.Multivac;
using Toggl.Multivac.Extensions;
using Toggl.PrimeRadiant;
using Toggl.PrimeRadiant.Models;

namespace Toggl.Foundation.DataSources
{
    public sealed class ProjectsDataSource : IProjectsSource
    {
        private readonly IIdProvider idProvider;
        private readonly ITimeService timeService;
        private readonly IRepository<IDatabaseProject> repository;

        public ProjectsDataSource(IIdProvider idProvider, IRepository<IDatabaseProject> repository, ITimeService timeService)
        {
            Ensure.Argument.IsNotNull(idProvider, nameof(idProvider));
            Ensure.Argument.IsNotNull(repository, nameof(repository));
            Ensure.Argument.IsNotNull(timeService, nameof(timeService));

            this.repository = repository;
            this.idProvider = idProvider;
            this.timeService = timeService;
        }

        public IObservable<IDatabaseProject> Create(CreateProjectDTO dto)
            => idProvider.GetNextIdentifier()
                .Apply(Project.Builder.Create)
                .SetName(dto.Name)
                .SetColor(dto.Color)
                .SetClientId(dto.ClientId)
                .SetBillable(dto.Billable)
                .SetWorkspaceId(dto.WorkspaceId)
                .SetAt(timeService.CurrentDateTime)
                .SetSyncStatus(SyncStatus.SyncNeeded)
                .Build()
                .Apply(repository.Create)
                .Select(Project.From);

        public IObservable<IEnumerable<IDatabaseProject>> GetAll()
            => repository.GetAll();

        public IObservable<IEnumerable<IDatabaseProject>> GetAll(Func<IDatabaseProject, bool> predicate)
            => repository.GetAll(predicate);

        public IObservable<IDatabaseProject> GetById(long id)
            => repository.GetById(id);
    }
}
