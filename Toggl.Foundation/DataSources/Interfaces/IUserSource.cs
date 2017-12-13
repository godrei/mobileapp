using System;
using System.Reactive;
using Toggl.PrimeRadiant.Models;

namespace Toggl.Foundation.DataSources
{
    public interface IUserSource
    {
        IObservable<IDatabaseUser> Current { get; }
        IObservable<IDatabaseUser> UpdateWorkspace(long workspaceId);
    }
}