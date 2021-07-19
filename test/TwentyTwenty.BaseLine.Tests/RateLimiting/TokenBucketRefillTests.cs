using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using TwentyTwenty.BaseLine.RateLimiting;
using Xunit;

namespace TwentyTwenty.BaseLine.Tests.RateLimiting
{
    public class TokenBucketRefillTests
    {
        [Fact]//, Explicit("Long Running")]
        public async Task RateLimitTests()
        {
            const int totalConsumes = 500;
            const int refillRate = 40;

            var tokenBucket = TokenBuckets.Construct()
                .WithCapacity(refillRate)
                .WithYieldingSleepStrategy()
                .WithFixedIntervalRefillStrategy(1, TimeSpan.FromMilliseconds(1000d / refillRate))
                .Build();

            var sw = new Stopwatch();
            sw.Start();
            for (var i = 0; i < totalConsumes; i++)
            {
                if (i % 3 == 0) Thread.Sleep(1000 / refillRate * 2);
                await tokenBucket.Consume();
            }

            sw.Stop();

            Assert.InRange(totalConsumes / (sw.Elapsed.TotalSeconds + 1), refillRate - 0.1, refillRate + 0.1);
        }
    }
}