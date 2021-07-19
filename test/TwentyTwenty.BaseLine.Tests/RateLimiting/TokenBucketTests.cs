using System;
using System.Diagnostics;
using System.Threading;
using TwentyTwenty.BaseLine.RateLimiting;
using Xunit;
using Moq;
using System.Threading.Tasks;

namespace TwentyTwenty.BaseLine.Tests.RateLimiting
{
    public class TokenBucketTests
    {
        private const long Capacity = 10;
        private const int ConsumeTimeout = 1000;

        private MockRefillStrategy _refillStrategy;
        private Mock<ISleepStrategy> _sleepStrategy;
        private ITokenBucket _bucket;

        public TokenBucketTests()
        {
            _refillStrategy = new MockRefillStrategy();
            _sleepStrategy = new Mock<ISleepStrategy>();
            _bucket = new TokenBucket(Capacity, _refillStrategy, _sleepStrategy.Object);
        }

        [Fact]
        public async Task TryConsumeZeroTokens()
        {
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _bucket.TryConsume(0));
        }

        [Fact]
        public async Task TryConsumeNegativeTokens()
        {
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _bucket.TryConsume(-1));
        }

        [Fact]
        public async Task TryConsumeMoreThanCapacityTokens()
        {
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _bucket.TryConsume(100));
        }

        [Fact]
        public async Task BucketInitiallyEmpty()
        {
            Assert.False(await _bucket.TryConsume());
        }

        [Fact]
        public async Task TryConsumeOneToken()
        {
            _refillStrategy.AddToken();
            Assert.True(await _bucket.TryConsume());
        }

        [Fact]
        public async Task TryConsumeMoreTokensThanAreAvailable()
        {
            _refillStrategy.AddToken();
            Assert.False(await _bucket.TryConsume(2));
        }

        [Fact]
        public async Task TryRefillMoreThanCapacityTokens()
        {
            _refillStrategy.AddTokens(Capacity + 1);
            Assert.True(await _bucket.TryConsume(Capacity));
            Assert.False(await _bucket.TryConsume(1));
        }

        [Fact]
        public async Task TryRefillWithTooManyTokens()
        {
            _refillStrategy.AddTokens(Capacity);
            Assert.True(await _bucket.TryConsume());

            _refillStrategy.AddTokens(long.MaxValue);
            Assert.True(await _bucket.TryConsume(Capacity));
            Assert.False(await _bucket.TryConsume(1));
        }

        [Fact(Timeout = ConsumeTimeout)]
        public async Task ConsumeWhenTokenAvailable()
        {
            _refillStrategy.AddToken();
            await _bucket.Consume();

            _sleepStrategy.Verify(s => s.Sleep(), Times.Never());
        }

        [Fact(Timeout = ConsumeTimeout)]
        public async Task ConsumeWhenTokensAvailable()
        {
            const int tokensToConsume = 2;
            _refillStrategy.AddTokens(tokensToConsume);
            await _bucket.Consume(tokensToConsume);

            _sleepStrategy.Verify(s => s.Sleep(), Times.Never());
        }

        [Fact(Timeout = ConsumeTimeout)]
        public async Task ConsumeWhenTokenUnavailable()
        {
            _sleepStrategy
                .Setup(s => s.Sleep())
                .Returns(Task.CompletedTask)
                .Callback(_refillStrategy.AddToken)
                .Verifiable();

            await _bucket.Consume();

            _sleepStrategy.Verify();
        }

        [Fact(Timeout = ConsumeTimeout)]
        public async Task ConsumeWhenTokensUnavailable()
        {
            const int tokensToConsume = 7;
            _sleepStrategy
                .Setup(s => s.Sleep())
                .Returns(Task.CompletedTask)
                .Callback(() => _refillStrategy.AddTokens(tokensToConsume))
                .Verifiable();

            await _bucket.Consume(tokensToConsume);

            _sleepStrategy.Verify();
        }

        private sealed class MockRefillStrategy : IRefillStrategy
        {
            private long _numTokensToAdd;

            public long Refill()
            {
                var numTokens = _numTokensToAdd;
                _numTokensToAdd = 0;
                return numTokens;
            }

            public void AddToken()
            {
                _numTokensToAdd++;
            }

            public void AddTokens(long numTokens)
            {
                _numTokensToAdd += numTokens;
            }

            public Task<long> RefillAsync()
            {
                var numTokens = _numTokensToAdd;
                _numTokensToAdd = 0;
                return Task.FromResult(numTokens);
            }
        }
    }
}