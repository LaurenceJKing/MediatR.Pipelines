using FluentAssertions;
using Mediatr.Pipelines.Caching;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Mediatr.Pipelines.Tests.Caching
{
    public class IdempotentRequestPipelineTests
    {
        [Fact]
        public async Task Given_new_key_then_response_should_be_cached()
        {
            var request = new IdempotentRequest(Guid.NewGuid().ToString());
            var cache = new MemoryCache(new MemoryCacheOptions());
            var pipeline = new IdempotentRequestPipeline<IdempotentRequest, Response>(
                cache);

            Task<Response> next() => Task.FromResult(new Response());

            await pipeline.Handle(request, default, next);

            cache.TryGetValue(request.IdempotencyKey, out var _).Should().BeTrue();
        }

        [Fact]
        public async Task Given_no_key_then_response_should_not_be_cached()
        {
            var request = new IdempotentRequest();
            var cache = new MemoryCache(new MemoryCacheOptions());
            var pipeline = new IdempotentRequestPipeline<IdempotentRequest, Response>(
                cache);

            Task<Response> next() => Task.FromResult(new Response());

            await pipeline.Handle(request, default, next);

            cache.TryGetValue(request.IdempotencyKey ?? "", out _).Should().BeFalse();
        }

        [Fact]
        public async Task Given_cached_key_then_response_should_come_from_cache()
        {
            var request = new IdempotentRequest(Guid.NewGuid().ToString());
            var cache = new MemoryCache(new MemoryCacheOptions());
            cache.GetOrCreate(request.IdempotencyKey, _ => new Response());

            var pipeline = new IdempotentRequestPipeline<IdempotentRequest, Response>(
                cache);

            var nextCalled = false;

            Task<Response> next()
            {
                nextCalled = true;
                return Task.FromResult(new Response());
            }

            await pipeline.Handle(request, default, next);

            nextCalled.Should().BeFalse();
        }
    }
}
