using Application.Abstractions.Caching;
using Microsoft.Extensions.Caching.Memory;

/* 
Pros and Cons of In-Memory Caching
Pros:
    Extremely fast
    Simple to implement
    No external dependencies
Cons:
    Cache data is lost if the server restarts
    Limited to the memory (RAM) of a single server
    Cache data is not shared across multiple instances of your application
*/

namespace Infrastructure.Caching;

internal sealed class MemoryCacheService(IMemoryCache memoryCache) : ICacheService
{
    public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        memoryCache.TryGetValue(key, out T? value);
        return Task.FromResult(value);
    }

    public Task SetAsync<T>(
        string key,
        T value,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default)
    {
        var options = new MemoryCacheEntryOptions();
        if (expiration.HasValue)
        {
            options.AbsoluteExpirationRelativeToNow = expiration;
        }

        memoryCache.Set(key, value, options);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        memoryCache.Remove(key);
        return Task.CompletedTask;
    }
}
