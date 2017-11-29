using System;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Toggl.Foundation
{
    public sealed class TimeService : ITimeService
    {
        private readonly CompositeDisposable disposable = new CompositeDisposable();

        private readonly IScheduler scheduler;

        private DateTimeOffset previousSecondLocalDateTime;

        private ISubject<DateTimeOffset> midnightSubject;

        public DateTimeOffset CurrentDateTime => floor(scheduler.Now);

        public IConnectableObservable<DateTimeOffset> CurrentDateTimeObservable { get; }

        public IConnectableObservable<DateTimeOffset> MidnightObservable { get; }

        public TimeService(IScheduler scheduler)
        {
            this.scheduler = scheduler;

            previousSecondLocalDateTime = CurrentDateTime.ToLocalTime();
            midnightSubject = new Subject<DateTimeOffset>();

            CurrentDateTimeObservable =
                Observable
                    .Interval(TimeSpan.FromSeconds(1), scheduler)
                    .Select(_ => CurrentDateTime)
                    .Do(tickOnMidnight)
                    .Publish();

            MidnightObservable = midnightSubject.AsObservable().Publish();

            disposable.Add(CurrentDateTimeObservable.Connect());
            disposable.Add(MidnightObservable.Connect());
        }

        private DateTimeOffset floor(DateTimeOffset t)
            => new DateTimeOffset(t.Year, t.Month, t.Day, t.Hour, t.Minute, t.Second, t.Offset);

        private void tickOnMidnight(DateTimeOffset now)
        {
            var localNow = now.ToLocalTime();
            if (localNow.Date != previousSecondLocalDateTime.Date)
                midnightSubject.OnNext(localNow);

            previousSecondLocalDateTime = now.ToLocalTime();
        }
    }
}
