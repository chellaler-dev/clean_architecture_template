using Application.Abstractions.Caching;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using SharedKernel;

namespace Application.Behaviors;

internal sealed class CachingBehavior<TRequest, TResponse>(
    IMemoryCache memoryCache,
    ILogger<CachingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICachedQuery
    where TResponse : Result
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        string cacheKey = request.CacheKey;
        string requestName = typeof(TRequest).Name;

        if (memoryCache.TryGetValue(cacheKey, out TResponse? cachedResult))
        {
            logger.LogInformation("Cache hit for {RequestName}", requestName);
            return cachedResult!;
        }

        logger.LogInformation("Cache miss for {RequestName}", requestName);

        TResponse result = await next();

        if (result.IsSuccess)
        {
            // Set cache options (e.g., absolute expiration)
            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = request.Expiration
            };

            memoryCache.Set(cacheKey, result, cacheEntryOptions);
        }

        return result;
    }
}
