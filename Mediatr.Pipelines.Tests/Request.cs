﻿using Mediatr.Pipelines.Caching;
using MediatR;
using System;

namespace Mediatr.Pipelines.Tests
{
    public class Request: IRequest<Response>
    {
    }

    public class CachedRequest : ICachedRequest<Response>
    {
        public string CacheKey { get; } = Guid.NewGuid().ToString();
    }

    public class IdempotentRequest : IIdempotentRequest<Response>
    {
        public string IdempotencyKey { get; }

        public IdempotentRequest(string idempotencyKey)
        {
            IdempotencyKey = idempotencyKey;
        }

        public IdempotentRequest(): this(null)
        {
        }
    }
}