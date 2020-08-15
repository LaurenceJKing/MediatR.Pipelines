using MediatR;
using Microsoft.Extensions.Caching.Memory;
using System.Threading;
using System.Threading.Tasks;

namespace Mediatr.Pipelines.Caching
{
    public abstract class CacheRequestPipelineBase<TRequest, TResponse> :
        IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IMemoryCache cache;

        protected CacheRequestPipelineBase(IMemoryCache cache)
        {
            this.cache = cache;
        }

        public Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            if (!TryGetCacheKey(request, out var cacheKey)) return next();
            return cache.GetOrCreateAsync(cacheKey, _ => next());
        }

        protected abstract bool TryGetCacheKey(TRequest rquest, out object cacheKey);
    }
}