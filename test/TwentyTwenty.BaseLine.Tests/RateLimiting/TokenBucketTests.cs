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
        public void TryConsumeZeroTokens()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                _bucket.TryConsume(0);
            });
        }

        [Fact]
        public void TryConsumeNegativeTokens()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                _bucket.TryConsume(-1);
            });
        }

        [Fact]
        public void TryConsumeMoreThanCapacityTokens()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                _bucket.TryConsume(100);
            });
        }

        [Fact]
        public void BucketInitiallyEmpty()
        {
            Assert.False(_bucket.TryConsume());
        }

        [Fact]
        public void TryConsumeOneToken()
        {
            _refillStrategy.AddToken();
            Assert.True(_bucket.TryConsume());
        }

        [Fact]
        public void TryConsumeMoreTokensThanAreAvailable()
        {
            _refillStrategy.AddToken();
            Assert.False(_bucket.TryConsume(2));
        }

        [Fact]
        public void TryRefillMoreThanCapacityTokens()
        {
            _refillStrategy.AddTokens(Capacity + 1);
            Assert.True(_bucket.TryConsume(Capacity));
            Assert.False(_bucket.TryConsume(1));
        }

        [Fact]
        public void TryRefillWithTooManyTokens()
        {
            _refillStrategy.AddTokens(Capacity);
            Assert.True(_bucket.TryConsume());

            _refillStrategy.AddTokens(long.MaxValue);
            Assert.True(_bucket.TryConsume(Capacity));
            Assert.False(_bucket.TryConsume(1));
        }

        [Fact(Timeout = ConsumeTimeout)]
        public void ConsumeWhenTokenAvailable()
        {
            _refillStrategy.AddToken();
            _bucket.Consume();

            _sleepStrategy.Verify(s => s.Sleep(), Times.Never());
        }

        [Fact(Timeout = ConsumeTimeout)]
        public void ConsumeAsyncWhenTokenAvailable()
        {
            _refillStrategy.AddToken();
            _bucket.ConsumeAsync().Wait();

            _sleepStrategy.Verify(s => s.Sleep(), Times.Never());
        }

        [Fact(Timeout = ConsumeTimeout)]
        public void ConsumeWhenTokensAvailable()
        {
            const int tokensToConsume = 2;
            _refillStrategy.AddTokens(tokensToConsume);
            _bucket.Consume(tokensToConsume);

            _sleepStrategy.Verify(s => s.Sleep(), Times.Never());
        }

        [Fact(Timeout = ConsumeTimeout)]
        public void ConsumeAsyncWhenTokensAvailable()
        {
            const int tokensToConsume = 2;
            _refillStrategy.AddTokens(tokensToConsume);
            _bucket.ConsumeAsync(tokensToConsume).Wait();

            _sleepStrategy.Verify(s => s.Sleep(), Times.Never());
        }

        [Fact(Timeout = ConsumeTimeout)]
        public void ConsumeWhenTokenUnavailable()
        {
            _sleepStrategy
                .Setup(s => s.Sleep())
                .Callback(_refillStrategy.AddToken)
                .Verifiable();

            _bucket.Consume();

            _sleepStrategy.Verify();
        }

        [Fact(Timeout = ConsumeTimeout)]
        public void ConsumeAsyncWhenTokenUnavailable()
        {
            _sleepStrategy
                .Setup(s => s.SleepAsync())
                .Returns(Task.CompletedTask)
                .Callback(_refillStrategy.AddToken)
                .Verifiable();

            _bucket.ConsumeAsync().Wait();
            _sleepStrategy.Verify();
        }

        [Fact(Timeout = ConsumeTimeout)]
        public void ConsumeWhenTokensUnavailable()
        {
            const int tokensToConsume = 7;
            _sleepStrategy
                .Setup(s => s.Sleep())
                .Callback(() => _refillStrategy.AddTokens(tokensToConsume))
                .Verifiable();

            _bucket.Consume(tokensToConsume);

            _sleepStrategy.Verify();
        }

        [Fact(Timeout = ConsumeTimeout)]
        public void ConsumeAsyncWhenTokensUnavailable()
        {
            const int tokensToConsume = 7;
            _sleepStrategy
                .Setup(s => s.SleepAsync())
                .Returns(Task.CompletedTask)
                .Callback(() => _refillStrategy.AddTokens(tokensToConsume))
                .Verifiable();

            _bucket.ConsumeAsync(tokensToConsume).Wait();

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