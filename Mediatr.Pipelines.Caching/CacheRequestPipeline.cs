using MediatR;
using Microsoft.Extensions.Caching.Memory;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Reflection;

namespace Mediatr.Pipelines.Caching
{
    public abstract class CacheRequestPipelineBase<TRquest, TResponse> :
        IPipelineBehavior<TRquest, TResponse>
        where TRquest : IRequest<TResponse>
    {
        private readonly IMemoryCache cache;

        protected CacheRequestPipelineBase(IMemoryCache cache)
        {
            this.cache = cache;
        }

        public Task<TResponse> Handle(TRquest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            if (!TryGetCacheKey(request, out var cacheKey)) return next();
            return cache.GetOrCreateAsync(cacheKey, _ => next());
        }

        protected abstract bool TryGetCacheKey(TRquest rquest, out object cacheKey);
    }


    public class CacheRequestPipeline<TRequest, TResponse> :
        CacheRequestPipelineBase<TRequest, TResponse>
        where TRequest : ICachedRequest<TResponse>
    {
        public CacheRequestPipeline(IMemoryCache memoryCache): base(memoryCache)
        {
        }

        protected override bool TryGetCacheKey(TRequest request, out object key)
        {
            key = request.CacheKey;
            return true;
        }
    }

    public interface ICachedRequest<T> : IRequest<T>
    {
        string CacheKey { get; }
    }
}