using MediatR;

namespace WingDing_Party.IdentityProvider.Application.Common.Behaviours;

// When to use: Cache responses for idempotent requests
public class CachingBehavior<TRequest, TResponse> 
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse> //, ICacheable
    //where TResponse : ...
{
    // private readonly ICacheService _cache;

    // ctor

    public async Task<TResponse> Handle(
        TRequest request, 
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        //if (request is ICachableRequest cachableRequest)
        //{
        //    var cacheKey = cachableRequest.CacheKey;
        //    var cached = await _cache.GetAsync<TResponse>(cacheKey);
        //    if (cached != null) return cached;

        //    var response = await next();
        //    await _cache.SetAsync(cacheKey, response, TimeSpan.FromMinutes(5));
        //    return response;
        //}
        return await next(cancellationToken);
    }
}
