using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using Toggl.Foundation.Autocomplete;
using Toggl.Foundation.Services;
using Toggl.Foundation.Sync;
using Toggl.Multivac;
using Toggl.Multivac.Extensions;
using Toggl.PrimeRadiant;
using Toggl.PrimeRadiant.Settings;

namespace Toggl.Foundation.DataSources
{
    public sealed class TogglDataSource : ITogglDataSource
    {
        private readonly ITogglDatabase database;
        private readonly IApiErrorHandlingService apiErrorHandlingService;

        private IDisposable errorHandlingDisposable;

        public TogglDataSource(
            ITogglDatabase database,
            ITimeService timeService,
            IApiErrorHandlingService apiErrorHandlingService,
            Func<ITogglDataSource, ISyncManager> createSyncManager)
        {
            Ensure.Argument.IsNotNull(database, nameof(database));
            Ensure.Argument.IsNotNull(timeService, nameof(timeService));
            Ensure.Argument.IsNotNull(apiErrorHandlingService, nameof(apiErrorHandlingService));
            Ensure.Argument.IsNotNull(createSyncManager, nameof(createSyncManager));

            this.database = database;
            this.apiErrorHandlingService = apiErrorHandlingService;

            User = new UserDataSource(database.User);
            Tags = new TagsDataSource(database.IdProvider, database.Tags, timeService);
            Tasks = new TasksDataSource(database.Tasks);
            Workspaces = new WorkspacesDataSource(database);
            Clients = new ClientsDataSource(database.IdProvider, database.Clients, timeService);
            Projects = new ProjectsDataSource(database.IdProvider, database.Projects, timeService);
            TimeEntries = new TimeEntriesDataSource(database.IdProvider, database.TimeEntries, timeService);

            AutocompleteProvider = new AutocompleteProvider(database);
            SyncManager = createSyncManager(this);

            errorHandlingDisposable = SyncManager.ProgressObservable.Subscribe(onSyncError);
        }

        public IUserSource User { get; }
        public ITagsSource Tags { get; }
        public ITasksSource Tasks { get; }
        public IClientsSource Clients { get; }
        public IProjectsSource Projects { get; }
        public IWorkspacesSource Workspaces { get; }
        public ITimeEntriesSource TimeEntries { get; }

        public ISyncManager SyncManager { get; }
        public IAutocompleteProvider AutocompleteProvider { get; }

        public IObservable<bool> HasUnsyncedData()
            => Observable.Merge(
                hasUnsyncedData(database.TimeEntries),
                hasUnsyncedData(database.Projects),
                hasUnsyncedData(database.User),
                hasUnsyncedData(database.Tasks),
                hasUnsyncedData(database.Clients),
                hasUnsyncedData(database.Tags),
                hasUnsyncedData(database.Workspaces))
                .Any(hasUnsynced => hasUnsynced);

        public IObservable<Unit> Logout()
            => SyncManager.Freeze()
                .FirstAsync()
                .SelectMany(_ => database.Clear())
                .FirstAsync();

        private IObservable<bool> hasUnsyncedData<TModel>(IRepository<TModel> repository)
            where TModel : IDatabaseSyncable
            => repository
                .GetAll(entity => entity.SyncStatus != SyncStatus.InSync)
                .Select(unsynced => unsynced.Any())
                .SingleAsync();

        private void onSyncError(Exception exception)
        {
            if (apiErrorHandlingService.TryHandleDeprecationError(exception) == false
                && apiErrorHandlingService.TryHandleUnauthorizedError(exception) == false)
            {
                throw new ArgumentException($"{nameof(TogglDataSource)} could not handle unknown sync error {exception.GetType().FullName}.", exception);
            }
        }
    }
}
