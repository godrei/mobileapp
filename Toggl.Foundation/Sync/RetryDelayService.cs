using System;

namespace Toggl.Foundation.Sync
{
    internal sealed class RetryDelayService : IRetryDelayService
    {
        private readonly Random random;

        private readonly TimeSpan? delayLimit;

        private double? lastDelay;

        private const double defaultFastDelay = 10;

        private const double defaultSlowDelay = 60;

        private double randomFastFactor => getRandomNumberBetween(1, 1.5);

        private double randomSlowFactor => getRandomNumberBetween(1.5, 2);

        public RetryDelayService(Random random, TimeSpan? delayLimit = null)
        {
            this.random = random;
            this.delayLimit = delayLimit;
            Reset();
        }

        public TimeSpan NextSlowDelay()
            => nextDelay(randomSlowFactor, defaultSlowDelay);

        public TimeSpan NextFastDelay()
            => nextDelay(randomFastFactor, defaultFastDelay);

        public void Reset() => lastDelay = null;

        private TimeSpan nextDelay(double factor, double defaultDelay)
        {
            lastDelay = !lastDelay.HasValue
                ? defaultDelay
                : Math.Max(Math.Min(lastDelay.Value * factor, TimeSpan.MaxValue.TotalSeconds), 0);

            var delay = TimeSpan.FromSeconds(lastDelay.Value);

            if (delayLimit.HasValue && delay > delayLimit)
                throw new TimeoutException($"Retry delay {delay.TotalSeconds}s exceeded the maximum delay limit {delayLimit.Value.TotalSeconds}s.");

            return delay;
        }

        private double getRandomNumberBetween(double min, double max)
            => random.NextDouble() * (max - min) + min;
    }
}
