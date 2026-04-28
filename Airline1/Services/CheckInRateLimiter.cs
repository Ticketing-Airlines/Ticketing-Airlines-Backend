using Microsoft.Extensions.Caching.Memory;

namespace Airline1.Services
{
    /// <summary>
    /// In-memory rate limiter for check-in operations.
    /// Tracks per-booking attempt counts for the complete endpoint.
    /// </summary>
    public class CheckInRateLimiter(IMemoryCache cache)
    {
        private readonly IMemoryCache _cache = cache;

        /// <summary>
        /// Checks if a complete check-in attempt is allowed for the given booking reference.
        /// Allows up to 3 attempts per booking per hour.
        /// </summary>
        public bool AllowComplete(string bookingReference)
        {
            var key = $"checkin_complete_{bookingReference.ToUpperInvariant()}";
            if (_cache.TryGetValue<int>(key, out var attempts))
            {
                if (attempts >= 3)
                    return false;
                _cache.Set(key, attempts + 1, TimeSpan.FromHours(1));
                return true;
            }

            _cache.Set(key, 1, TimeSpan.FromHours(1));
            return true;
        }
    }
}
