using AutoFixture.Xunit2;
using FluentAssertions;
using Mediatr.Pipelines.Caching;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Mediatr.Pipelines.Tests.Caching
{
    public class CacheRequestPipelineTests
    {
        [Theory]
        [AutoData]
        public async Task Given_new_key_then_response_should_be_cached(
            CachedRequest request)
        {
            var cache = new MemoryCache(new MemoryCacheOptions());
            var pipeline = new CacheRequestPipeline<CachedRequest, Response>(
                cache);

            Task<Response> next() => Task.FromResult(new Response());

            await pipeline.Handle(request, default, next);

            cache.TryGetValue(request.CacheKey, out var _).Should().BeTrue();
        }

        [Theory]
        [AutoData]
        public async Task Given_cached_key_then_response_should_come_from_cache(
            CachedRequest request)
        {
            var cache = new MemoryCache(new MemoryCacheOptions());
            cache.GetOrCreate(request.CacheKey, _ => new Response());

            var pipeline = new CacheRequestPipeline<CachedRequest, Response>(
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
