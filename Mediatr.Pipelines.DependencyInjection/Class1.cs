using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Mediatr.Pipelines.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddHandler<THandler, TRequest, TResponse>(
            this IServiceCollection services,
            ServiceLifetime lifetime,
            Func<
                HandlerRegistrationBuilder<THandler, TRequest, TResponse>,
                HandlerRegistrationBuilder<THandler, TRequest, TResponse>> configurePipeline = null)
            where THandler : class, IRequestHandler<TRequest, TResponse>
            where TRequest : IRequest<TResponse> {
            var builder = new HandlerRegistrationBuilder<THandler, TRequest, TResponse>(
                services,
                lifetime);
            configurePipeline?.Invoke(builder);
            return services;
            
        
        }

        public static IServiceCollection Add<TService, TImplementation>(
            this IServiceCollection services,
            ServiceLifetime lifetime)
            where TService:class
            where TImplementation:class, TService
        {
            switch (lifetime)
            {
                case ServiceLifetime.Transient:
                    return services.AddTransient<TService, TImplementation>();
                case ServiceLifetime.Scoped:
                    return services.AddScoped<TService, TImplementation>();
                case ServiceLifetime.Singleton:
                    return services.AddSingleton<TService, TImplementation>();
            }
            return services;
        }

        public static IServiceCollection Add<TService>(
            this IServiceCollection services,
            ServiceLifetime lifetime)
            where TService : class
        {
            switch (lifetime)
            {
                case ServiceLifetime.Transient:
                    return services.AddTransient<TService>();
                case ServiceLifetime.Scoped:
                    return services.AddScoped<TService>();
                case ServiceLifetime.Singleton:
                    return services.AddSingleton<TService>();
            }
            return services;
        }
    }

    public class HandlerRegistrationBuilder<THandler, TRequest, TResponse>
        where THandler: class, IRequestHandler<TRequest, TResponse>
        where TRequest: IRequest<TResponse>
    {
        private readonly IServiceCollection services;
        private readonly ServiceLifetime lifetime;

        internal HandlerRegistrationBuilder(
            IServiceCollection services,
            ServiceLifetime lifetime)
        {
            this.services = services.Add<IRequestHandler<TRequest, TResponse>, THandler>(lifetime);
            this.lifetime = lifetime;
        }

        public HandlerRegistrationBuilder<THandler, TRequest, TResponse>
            WithPipeline<TPipeline>() where TPipeline: class, IPipelineBehavior<TRequest, TResponse>
        {
           switch (lifetime) {
                case ServiceLifetime.Transient:
                    services.AddTransient<TPipeline>();
                    break;
                case ServiceLifetime.Scoped:
                    services.AddScoped<TPipeline>();
                    break;
                case ServiceLifetime.Singleton: 
                    services.AddSingleton<TPipeline>();
                    break;
            }

            return this;
        }

        public class Example
        {
            public void Foo(IServiceCollection services)
            {
                services
                    .AddHandler<T, Request, int>(
                    ServiceLifetime.Scoped,
                    _=> _.WithPipeline<Pipeline>());
            }
        }

        public class Request: IRequest<int> { }
        public class T : IRequestHandler<Request, int>
        {
            public Task<int> Handle(Request request, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }
        }

        public class Pipeline : IPipelineBehavior<Request, int>
        {
            public Task<int> Handle(Request request, CancellationToken cancellationToken, RequestHandlerDelegate<int> next)
            {
                throw new NotImplementedException();
            }
        }
    }
}
