using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Toggl.Multivac;

namespace Toggl.Foundation.Services
{
    public sealed class BackgroundService : IBackgroundService
    {
        private readonly ITimeService timeService;
        private readonly TimeSpan timeInBackground;

        private DateTimeOffset? lastEnteredBackground { get; set; }
        private ISubject<TimeSpan> appBecameActiveSubject { get; }

        public BackgroundService(ITimeService timeService, TimeSpan timeInBackground)
        {
            Ensure.Argument.IsNotNull(timeService, nameof(timeService));

            this.timeService = timeService;
            this.timeInBackground = timeInBackground;

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
            => appBecameActiveSubject
                .AsObservable()
                .Where(actualTimeSpentInBackground => actualTimeSpentInBackground >= timeInBackground)
                .Select(_ => Unit.Default);
    }
}
