using ExactOnline.Client.Sdk.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExactOnline.Client.Sdk.Interfaces
{
    /// <summary>
    /// Throttler to prevent rate limiting errors
    /// </summary>
    public interface IExactOnlineThrottler
    {
        /// <summary>
        /// Waits till when a next request can be done
        /// </summary>
        /// <returns></returns>
        object GetThrottleLock();

        /// <summary>
        /// Sets the rate limit headers
        /// </summary>
        /// <param name="rateLimit"></param>
        void SetRateLimitHeaders(RateLimit rateLimit);
    }
}
