using MediatR;
using Microsoft.Extensions.Caching.Memory;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Mediatr.Pipelines.Caching
{
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

    public interface IIdempotentRequest<T> : IRequest<T>
    {
        string IdempotencyKey { get; }
    }
}
