using System;
using System.Diagnostics;
using System.Threading;
using TwentyTwenty.BaseLine.RateLimiting;
using Xunit;

namespace TwentyTwenty.BaseLine.Tests.RateLimiting
{
    public class TokenBucketsBuilderTests
    {
        private readonly TokenBuckets.Builder _builder = TokenBuckets.Construct();

        [Fact]
        public void WithNegativeCapacity()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => 
            {
                _builder.WithCapacity(-1);
            });
        }

        [Fact]
        public void WithZeroCapacity()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => 
            {
                _builder.WithCapacity(0);
            });
        }

        [Fact]
        public void WithNullRefillStrategy()
        {
            Assert.Throws<ArgumentNullException>(() => 
            {
                _builder.WithRefillStrategy(null);
            });
        }

        [Fact]
        public void WithNullSleepStrategy()
        {
            Assert.Throws<ArgumentNullException>(() => 
            {
                _builder.WithSleepStrategy(null);
            });
        }

        [Fact]
        public void BuildWhenCapacityNotSpecified()
        {
            Assert.Throws<InvalidOperationException>(() => 
            {
                _builder.Build();
            });
        }
    }
}