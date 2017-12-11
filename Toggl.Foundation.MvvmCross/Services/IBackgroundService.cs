using System;
using System.Reactive;

namespace Toggl.Foundation.MvvmCross.Services
{
    public interface IBackgroundService
    {
        void EnterBackground();
        void EnterForeground();

        IObservable<Unit> AppBecameActiveAfterAtLeast(TimeSpan timeInBackground);
    }
}
