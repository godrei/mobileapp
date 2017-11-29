using System;
using System.Reactive.Subjects;

namespace Toggl.Foundation
{
    public interface ITimeService
    {
        DateTimeOffset CurrentDateTime { get; }

        IConnectableObservable<DateTimeOffset> CurrentDateTimeObservable { get; } 

        IConnectableObservable<DateTimeOffset> MidnightObservable { get; }
    }
}
