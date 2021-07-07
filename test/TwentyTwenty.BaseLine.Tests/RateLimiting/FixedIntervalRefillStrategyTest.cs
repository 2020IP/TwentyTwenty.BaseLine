using System;
using TwentyTwenty.BaseLine.RateLimiting;
using Xunit;

namespace TwentyTwenty.BaseLine.Tests.RateLimiting
{
    public class FixedIntervalRefillStrategyTest
    {
        private const long NumberOfTokens = 5;
        private readonly TimeSpan _period = TimeSpan.FromSeconds(10);

        private MockTicker _ticker;
        private FixedIntervalRefillStrategy _strategy;

        public FixedIntervalRefillStrategyTest()
        {
            _ticker = new MockTicker();
            _strategy = new FixedIntervalRefillStrategy(_ticker, NumberOfTokens, _period);
        }

        [Fact]
        public void FirstRefill()
        {
            Assert.Equal(NumberOfTokens, _strategy.Refill());
        }

        [Fact]
        public void NoRefillUntilPeriodUp()
        {
            _strategy.Refill();

            // Another refill shouldn't come for P units.
            for (var i = 0; i < _period.TotalSeconds - 1; i++)
            {
                _ticker.Advance(TimeSpan.FromSeconds(1));
                Assert.Equal(0, _strategy.Refill());
            }
        }

        [Fact]
        public void RefillEveryPeriod()
        {
            for (var i = 0; i < 10; i++)
            {
                Assert.Equal(NumberOfTokens, _strategy.Refill());
                _ticker.Advance(_period);
            }
        }

        [Fact]
        public void RefillMultipleTokensWhenMultiplePeriodsElapse()
        {
            _ticker.Advance(TimeSpan.FromSeconds(_period.TotalSeconds * 3));
            Assert.Equal(NumberOfTokens * 3, _strategy.Refill());

            _ticker.Advance(_period);
            Assert.Equal(NumberOfTokens, _strategy.Refill());
        }

        [Fact]
        public void RefillAtFixedRateWhenCalledWithInconsistentRate()
        {
            _ticker.Advance(TimeSpan.FromSeconds(_period.TotalSeconds / 2));
            Assert.Equal(NumberOfTokens, _strategy.Refill());

            _ticker.Advance(TimeSpan.FromSeconds(_period.TotalSeconds / 2));
            Assert.Equal(NumberOfTokens, _strategy.Refill());
        }

        private sealed class MockTicker : Ticker
        {
            private long _now;

            public override long Read()
            {
                return _now;
            }

            public void Advance(TimeSpan delta)
            {
                _now += delta.Ticks;
            }
        }
    }
}