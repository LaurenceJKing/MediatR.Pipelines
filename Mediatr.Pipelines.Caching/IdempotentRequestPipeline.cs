using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace Mediatr.Pipelines.Caching
{
    /// <summary>
    /// A MediatR pipeline that will cache the response if the <see cref="IIdempotentRequest{T}.IdempotencyKey"/> is set.
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    public class IdempotentRequestPipeline<TRequest, TResponse> :
        CacheRequestPipelineBase<TRequest, TResponse>
        where TRequest : IIdempotentRequest<TResponse>
    {
        public IdempotentRequestPipeline(IMemoryCache memoryCache):base(memoryCache)
        {
        }

        protected override bool TryGetCacheKey(TRequest request, out object key)
        {
            key = request.IdempotencyKey;
            return key != null;
        }
    }

    /// <summary>
    /// A marker interface to represent a request with a response and an optional <see cref="IdempotencyKey"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IIdempotentRequest<T> : IRequest<T>
    {
        string IdempotencyKey { get; }
    }
}
