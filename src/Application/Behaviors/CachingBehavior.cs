using Application.Abstractions.Caching;
using MediatR;
using Microsoft.Extensions.Logging;
using SharedKernel;

namespace Application.Behaviors;

internal sealed class CachingBehavior<TRequest, TResponse>(
    ICacheService cacheService,
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

        // Try to get from cache
        TResponse? cachedResult = await cacheService.GetAsync<TResponse>(cacheKey, cancellationToken);
        if (cachedResult is not null)
        {
            logger.LogInformation("Cache hit for {RequestName}", requestName);
            return cachedResult;
        }

        logger.LogInformation("Cache miss for {RequestName}", requestName);

        TResponse result = await next();

        if (result.IsSuccess)
        {
            await cacheService.SetAsync(cacheKey, result, request.Expiration, cancellationToken);
        }

        return result;
    }
}
