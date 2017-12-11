using System;
using System.Reactive;

namespace Toggl.Foundation.Services
{
    public interface IBackgroundService
    {
        void EnterBackground();
        void EnterForeground();

        IObservable<Unit> AppBecameActive { get; }
        IObservable<Unit> AppBecameActiveAfterAtLeast(TimeSpan timeInBackground);
    }
}
