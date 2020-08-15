using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace Mediatr.Pipelines.Caching
{
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