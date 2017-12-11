using System;
using FluentAssertions;
using FsCheck;
using NSubstitute;
using Toggl.Foundation.MvvmCross.Services;
using Xunit;
using FsCheck.Xunit;

namespace Toggl.Foundation.Tests.MvvmCross.Services
{
    public sealed class BackgroundServiceTests
    {
        public abstract class BackgroundServiceTest
        {
            protected readonly ITimeService TimeService;
            protected readonly IBackgroundService BackgroundService;

            public BackgroundServiceTest()
            {
                TimeService = Substitute.For<ITimeService>();
                BackgroundService = new BackgroundService(TimeService);
            }
        }

        public sealed class TheConstructor
        {
            [Fact]
            public void ThrowsWhenTheArgumentIsNull()
            {
                Action constructor = () => new BackgroundService(null);

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
                BackgroundService
                    .AppBecameActiveAfterAtLeast(TimeSpan.Zero)
                    .Subscribe(_ => emitted = true);

                BackgroundService.EnterForeground();

                emitted.Should().BeFalse();
            }

            [Fact]
            public void EmitsValueWhenEnteringForegroundAfterBeingInBackground()
            {
                bool emitted = false;
                TimeService.CurrentDateTime.Returns(now);
                BackgroundService
                    .AppBecameActiveAfterAtLeast(TimeSpan.Zero)
                    .Subscribe(_ => emitted = true);

                BackgroundService.EnterBackground();
                BackgroundService.EnterForeground();

                emitted.Should().BeTrue();
            }

            [Fact]
            public void DoesNotEmitAnythingWhenTheEnterForegroundIsCalledMultipleTimes()
            {
                bool emitted = false;
                TimeService.CurrentDateTime.Returns(now);
                BackgroundService.EnterBackground();
                TimeService.CurrentDateTime.Returns(now.Add(limit).AddMinutes(1));
                BackgroundService.EnterForeground();
                TimeService.CurrentDateTime.Returns(now.Add(limit).AddMinutes(1).Add(limit).AddMinutes(1));
                BackgroundService
                    .AppBecameActiveAfterAtLeast(limit)
                    .Subscribe(_ => emitted = true);

                BackgroundService.EnterForeground();

                emitted.Should().BeFalse();
            }

            [Property]
            public void EmitsAValueWhenEnteringForegroundAfterBeingInBackgroundForMoreThanTheLimit(NonNegativeInt limit, NonNegativeInt extraWaitingTime)
            {
                bool emitted = false;
                BackgroundService
                    .AppBecameActiveAfterAtLeast(TimeSpan.FromMinutes(limit.Get))
                    .Subscribe(_ => emitted = true);
                TimeService.CurrentDateTime.Returns(now);

                BackgroundService.EnterBackground();
                TimeService.CurrentDateTime.Returns(now.AddMinutes(limit.Get).AddMinutes(extraWaitingTime.Get));
                BackgroundService.EnterForeground();

                emitted.Should().BeTrue();
            }

            [Property]
            public void DoesNotEmitAnyValueWhenEnteringForegroundAfterBeingInBackgroundForLessThanTheLimit(NonNegativeInt a, NonNegativeInt b)
            {
                if (a.Get == b.Get) return;

                var limit = Math.Max(a.Get, b.Get);
                var waitingTime = Math.Min(a.Get, b.Get);

                bool emitted = false;
                BackgroundService
                    .AppBecameActiveAfterAtLeast(TimeSpan.FromMinutes(limit))
                    .Subscribe(_ => emitted = true);
                TimeService.CurrentDateTime.Returns(now);

                BackgroundService.EnterBackground();
                TimeService.CurrentDateTime.Returns(now.AddMinutes(waitingTime));
                BackgroundService.EnterForeground();

                emitted.Should().BeFalse();
            }
        }
    }
}
