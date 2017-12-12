using System;
using FluentAssertions;
using FsCheck;
using NSubstitute;
using Xunit;
using FsCheck.Xunit;
using Toggl.Foundation.Services;

namespace Toggl.Foundation.Tests.Services
{
    public sealed class BackgroundServiceTests
    {
        public abstract class BackgroundServiceTest
        {
            protected readonly ITimeService TimeService;

            public BackgroundServiceTest()
            {
                TimeService = Substitute.For<ITimeService>();
            }
        }

        public sealed class TheConstructor
        {
            [Fact]
            public void ThrowsWhenTheArgumentIsNull()
            {
                Action constructor = () => new BackgroundService(null, TimeSpan.Zero);

                constructor.ShouldThrow<ArgumentNullException>();
            }
        }

        public sealed class TheAppBecameActiveAfterAtLeastMethod : BackgroundServiceTest
        {
            private readonly DateTimeOffset now = new DateTimeOffset(2017, 12, 11, 0, 30, 59, TimeSpan.Zero);
            private readonly TimeSpan limit = TimeSpan.FromMinutes(2);

            [Fact]
            public void DoesNotEmitAnythingWhenItHasNotEnterBackgroundFirst()
            {
                bool emitted = false;
                var backgroundService = new BackgroundService(TimeService, TimeSpan.Zero);
                backgroundService
                    .AppBecameActive
                    .Subscribe(_ => emitted = true);

                backgroundService.EnterForeground();

                emitted.Should().BeFalse();
            }

            [Fact]
            public void EmitsValueWhenEnteringForegroundAfterBeingInBackground()
            {
                bool emitted = false;
                var backgroundService = new BackgroundService(TimeService, TimeSpan.Zero);
                TimeService.CurrentDateTime.Returns(now);
                backgroundService
                    .AppBecameActive
                    .Subscribe(_ => emitted = true);

                backgroundService.EnterBackground();
                backgroundService.EnterForeground();

                emitted.Should().BeTrue();
            }

            [Fact]
            public void DoesNotEmitAnythingWhenTheEnterForegroundIsCalledMultipleTimes()
            {
                bool emitted = false;
                var backgroundService = new BackgroundService(TimeService, limit);
                TimeService.CurrentDateTime.Returns(now);
                backgroundService.EnterBackground();
                TimeService.CurrentDateTime.Returns(now.Add(limit).AddMinutes(1));
                backgroundService.EnterForeground();
                TimeService.CurrentDateTime.Returns(now.Add(limit).AddMinutes(1).Add(limit).AddMinutes(1));
                backgroundService
                    .AppBecameActive
                    .Subscribe(_ => emitted = true);

                backgroundService.EnterForeground();

                emitted.Should().BeFalse();
            }

            [Property]
            public void EmitsAValueWhenEnteringForegroundAfterBeingInBackgroundForMoreThanTheLimit(NonNegativeInt limit, NonNegativeInt extraWaitingTime)
            {
                bool emitted = false;
                var backgroundService = new BackgroundService(TimeService, TimeSpan.FromMinutes(limit.Get));
                backgroundService
                    .AppBecameActive
                    .Subscribe(_ => emitted = true);
                TimeService.CurrentDateTime.Returns(now);

                backgroundService.EnterBackground();
                TimeService.CurrentDateTime.Returns(now.AddMinutes(limit.Get).AddMinutes(extraWaitingTime.Get));
                backgroundService.EnterForeground();

                emitted.Should().BeTrue();
            }

            [Property]
            public void DoesNotEmitAnyValueWhenEnteringForegroundAfterBeingInBackgroundForLessThanTheLimit(NonNegativeInt a, NonNegativeInt b)
            {
                if (a.Get == b.Get) return;

                var limit = Math.Max(a.Get, b.Get);
                var waitingTime = Math.Min(a.Get, b.Get);
                var backgroundService = new BackgroundService(TimeService, TimeSpan.FromMinutes(limit));
                bool emitted = false;
                backgroundService
                    .AppBecameActive
                    .Subscribe(_ => emitted = true);
                TimeService.CurrentDateTime.Returns(now);

                backgroundService.EnterBackground();
                TimeService.CurrentDateTime.Returns(now.AddMinutes(waitingTime));
                backgroundService.EnterForeground();

                emitted.Should().BeFalse();
            }
        }
    }
}
