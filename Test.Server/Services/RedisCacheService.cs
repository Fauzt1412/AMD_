using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace ShortenURLService.Services
{
    //This defines a contract (interface) for Redis
    public interface IRedisCacheService
    {
        Task CacheUrlAsync(string ShortenedUrl, string OriginalUrl);
        Task<string?> GetCachedUrlAsync(string ShortenedUrl);
    }

    public class RedisCacheService : IRedisCacheService
    {
        //Constructor and Redis Connection
        private readonly IDatabase _cache;

        public RedisCacheService(IConnectionMultiplexer redis)
        {
            _cache = redis.GetDatabase();
        }

        //Cache a Shortened URL
        public async Task CacheUrlAsync(string ShortenedUrl, string OriginalUrl)
        {
            await _cache.StringSetAsync(ShortenedUrl, OriginalUrl, TimeSpan.FromHours(1)); // Cache for 1 hour
        }

        //Retrieve Cached URL
        public async Task<string?> GetCachedUrlAsync(string ShortenedUrl)
        {
            return await _cache.StringGetAsync(ShortenedUrl);
        }
    }

    // Mock implementation for when Redis is not available
    public class MockRedisCacheService : IRedisCacheService
    {
        private readonly ConcurrentDictionary<string, (string url, DateTime expiry)> _cache = new();

        public Task CacheUrlAsync(string ShortenedUrl, string OriginalUrl)
        {
            _cache.TryAdd(ShortenedUrl, (OriginalUrl, DateTime.UtcNow.AddHours(1)));
            return Task.CompletedTask;
        }

        public Task<string?> GetCachedUrlAsync(string ShortenedUrl)
        {
            if (_cache.TryGetValue(ShortenedUrl, out var cached))
            {
                if (cached.expiry > DateTime.UtcNow)
                {
                    return Task.FromResult<string?>(cached.url);
                }
                else
                {
                    _cache.TryRemove(ShortenedUrl, out _);
                }
            }
            return Task.FromResult<string?>(null);
        }
    }
}