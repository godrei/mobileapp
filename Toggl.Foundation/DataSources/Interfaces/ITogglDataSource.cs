﻿using System;
using System.Reactive;
using Toggl.Foundation.Autocomplete;
using Toggl.Foundation.Sync;

namespace Toggl.Foundation.DataSources
{
    public interface ITogglDataSource
    {
        ITagsSource Tags { get; }
        IUserSource User { get; }
        ITasksSource Tasks { get; }
        IClientsSource Clients { get; }
        IProjectsSource Projects { get; }
        IWorkspacesSource Workspaces { get; }
        ITimeEntriesSource TimeEntries { get; }

        ISyncManager SyncManager { get; }
        IAutocompleteProvider AutocompleteProvider { get; }

        IObservable<bool> HasUnsyncedData();
        IObservable<Unit> Logout();
    }
}
