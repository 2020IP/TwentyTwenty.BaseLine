using System.Threading;
using System.Threading.Tasks;

namespace TwentyTwenty.BaseLine.RateLimiting
{
    /// <summary>
    /// A token bucket is used for rate limiting access to a portion of code.
    /// 
    /// See <a href="http://en.wikipedia.org/wiki/Token_bucket">Token Bucket on Wikipedia</a>
    /// See <a href="http://en.wikipedia.org/wiki/Leaky_bucket">Leaky Bucket on Wikipedia</a>
    /// </summary>
    public interface ITokenBucket
    {
        /// <summary>
        /// Attempt to consume a single token from the bucket.  If it was consumed then <code>true</code>
        /// is returned, otherwise <code>false</code> is returned.
        /// </summary>
        /// <returns><code>true</code> if the tokens were consumed, <code>false</code> otherwise.</returns>
        Task<bool> TryConsume();

        /// <summary>
        /// Attempt to consume a specified number of tokens from the bucket.  If the tokens were consumed then <code>true</code>
        /// is returned, otherwise <code>false</code> is returned.
        /// </summary>
        /// <param name="numTokens">The number of tokens to consume from the bucket, must be a positive number.</param>
        /// <returns><code>true</code> if the tokens were consumed, <code>false</code> otherwise.</returns>
        Task<bool> TryConsume(long numTokens);

        /// <summary>
        /// Consume a single token from the bucket asynchronously. This method does not block
        /// <returns>A task that returns once a token has been consumed</returns>
        /// </summary>
        Task Consume(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Consume multiple tokens from the bucket asynchronously. This method does not block
        /// <param name="numTokens">The number of tokens to consume from the bucket, must be a positive number.</param>
        /// <returns>A task that returns once the requested tokens have been consumed</returns>
        /// </summary>
        Task Consume(long numTokens, CancellationToken cancellationToken = default(CancellationToken));
    }
}