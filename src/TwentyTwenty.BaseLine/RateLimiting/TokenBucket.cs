using System;
using System.Threading;
using System.Threading.Tasks;

namespace TwentyTwenty.BaseLine.RateLimiting
{
    /// <summary>
    /// A token bucket implementation that is of a leaky bucket in the sense that it has a finite capacity and any added
    /// tokens that would exceed this capacity will "overflow" out of the bucket and are lost forever.
    ///
    /// In this implementation the rules for refilling the bucket are encapsulated in a provided <see cref="IRefillStrategy"/>
    /// instance.  Prior to attempting to consume any tokens the refill strategy will be consulted to see how many tokens
    /// should be added to the bucket.
    ///
    /// In addition in this implementation the method of yielding CPU control is encapsulated in the provided
    /// <see cref="ISleepStrategy"/> instance.  For high performance applications where tokens are being refilled incredibly quickly
    /// and an accurate bucket implementation is required, it may be useful to never yield control of the CPU and to instead
    /// busy wait.  This strategy allows the caller to make this decision for themselves instead of the library forcing a
    /// decision.
    /// </summary>
    internal class TokenBucket : ITokenBucket
    {
        private readonly long _capacity;
        private readonly IRefillStrategy _refillStrategy;
        private readonly ISleepStrategy _sleepStrategy;
        private long _size;
        private readonly SemaphoreSlim _mutex = new SemaphoreSlim(1, 1);

        public TokenBucket(long capacity, IRefillStrategy refillStrategy, ISleepStrategy sleepStrategy)
        {
            _capacity = capacity;
            _refillStrategy = refillStrategy;
            _sleepStrategy = sleepStrategy;
            _size = 0;
        }

        public Task<bool> TryConsume()
            => TryConsume(1);

        public async Task<bool> TryConsume(long numTokens)
        {
            if (numTokens <= 0)
                throw new ArgumentOutOfRangeException("numTokens", "Number of tokens to consume must be positive");
            if (numTokens > _capacity)
                throw new ArgumentOutOfRangeException("numTokens", "Number of tokens to consume must be less than the capacity of the bucket.");

            await _mutex.WaitAsync();
            try
            {
                // Give the refill strategy a chance to add tokens if it needs to, but beware of overflow
                var refilledTokens = await _refillStrategy.RefillAsync().ConfigureAwait(false);
                var newTokens = Math.Min(_capacity, Math.Max(0, refilledTokens));
                _size = Math.Max(0, Math.Min(_size + newTokens, _capacity));

                if (numTokens > _size) return false;

                // Now try to consume some tokens
                _size -= numTokens;
                return true;
            }
            finally
            {
                _mutex.Release();
            }
        }

        /// <summary>
        /// Consume a single token from the bucket asynchronously. This method does not block
        /// <returns>A task that returns once a token has been consumed</returns>
        /// </summary>
        public Task Consume() => Consume(1);

        /// <summary>
        /// Consume multiple tokens from the bucket asynchronously. This method does not block
        /// <param name="numTokens">The number of tokens to consume from the bucket, must be a positive number.</param>
        /// <returns>A task that returns once the requested tokens have been consumed</returns>
        /// </summary>
        public async Task Consume(long numTokens)
        {
            while (true) 
            {
                if (await TryConsume(numTokens).ConfigureAwait(false))
                {
                    break;
                }

                await _sleepStrategy.Sleep().ConfigureAwait(false);
            }
        }
    }
}