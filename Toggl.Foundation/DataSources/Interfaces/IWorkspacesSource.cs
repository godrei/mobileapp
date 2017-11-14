using System;
using System.Collections.Generic;
using Toggl.Multivac;
using Toggl.PrimeRadiant.Models;

namespace Toggl.Foundation.DataSources
{
    public interface IWorkspacesSource
    {
        IObservable<IDatabaseWorkspace> GetDefault();

        IObservable<IEnumerable<IDatabaseWorkspace>> GetAll();

        IObservable<IDatabaseWorkspace> GetById(long id);

        IObservable<bool> WorkspaceHasFeature(long workspaceId, WorkspaceFeatureId feature);
    }
}