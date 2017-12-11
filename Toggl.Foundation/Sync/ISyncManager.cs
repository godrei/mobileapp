using System;
using System.Reactive;

namespace Toggl.Foundation.Sync
{
    public interface ISyncManager
    {
        SyncState State { get; }
        IObservable<SyncProgress> ProgressObservable { get; }
        bool IsRunningSync { get; }

        IObservable<SyncState> PushSync();
        IObservable<SyncState> ForceFullSync();
        void ForceFullSyncOnSignal(IObservable<Unit> signalSource);

        IObservable<SyncState> Freeze();
    }
}
