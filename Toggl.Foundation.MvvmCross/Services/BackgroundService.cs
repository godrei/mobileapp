using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Toggl.Multivac;

namespace Toggl.Foundation.MvvmCross.Services
{
    public sealed class BackgroundService : IBackgroundService
    {
        private readonly ITimeService timeService;
        private readonly TimeSpan defaultTimeInBackground;

        private DateTimeOffset? lastEnteredBackground { get; set; }
        private ISubject<TimeSpan> appBecameActiveSubject { get; }

        public BackgroundService(ITimeService timeService, TimeSpan defaultTimeInBackground)
        {
            Ensure.Argument.IsNotNull(timeService, nameof(timeService));

            this.timeService = timeService;
            this.defaultTimeInBackground = defaultTimeInBackground;

            appBecameActiveSubject = new Subject<TimeSpan>();
            lastEnteredBackground = null;
        }

        public void EnterBackground()
            => lastEnteredBackground = timeService.CurrentDateTime;

        public void EnterForeground()
        {
            if (lastEnteredBackground.HasValue == false)
                return;

            var timeInBackground = timeService.CurrentDateTime - lastEnteredBackground.Value;
            lastEnteredBackground = null;
            appBecameActiveSubject.OnNext(timeInBackground);
        }

        public IObservable<Unit> AppBecameActive
            => AppBecameActiveAfterAtLeast(defaultTimeInBackground);

        public IObservable<Unit> AppBecameActiveAfterAtLeast(TimeSpan timeInBackground)
            => appBecameActiveSubject
                .AsObservable()
                .Where(actualTimeSpentInBackground => actualTimeSpentInBackground >= timeInBackground)
                .Select(_ => Unit.Default);
    }
}
