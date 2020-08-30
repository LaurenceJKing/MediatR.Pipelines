using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Mediatr.Pipelines.DependencyInjection.NetCore
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection Add<TService, TImplementation>(
            this IServiceCollection services,
            ServiceLifetime lifetime)
            where TImplementation : class, TService
            where TService : class
        {
            switch (lifetime)
            {
                case ServiceLifetime.Scoped:
                    return services.AddScoped<TService, TImplementation>();
                case ServiceLifetime.Singleton:
                    return services.AddSingleton<TService, TImplementation>();
                case ServiceLifetime.Transient:
                    return services.AddTransient<TService, TImplementation>();
                default:
                    throw new InvalidOperationException($"{lifetime} not recognised. Service could not be registered.");
            }
        }

        public static IServiceCollection AddHandler<THandler, TRequest, TResponse>(
            this IServiceCollection services,
            ServiceLifetime lifetime,
            Action<MediatrPipelineBuilder<THandler, TRequest, TResponse>> addPipelines = null)
            where THandler : class, IRequestHandler<TRequest, TResponse>
            where TRequest : IRequest<TResponse>
        {
            services.Add<IRequestHandler<TRequest, TResponse>, THandler>(lifetime);

            if (addPipelines is null) return services;

            var builder = new MediatrPipelineBuilder<THandler, TRequest, TResponse>(
                services,
                lifetime);

            addPipelines(builder);

            return services;
        }

        public static IServiceCollection AddScoped<THandler, TRequest, TResponse>(
            this IServiceCollection services,
            Action<MediatrPipelineBuilder<THandler, TRequest, TResponse>> addPipelines = null)
                where THandler : class, IRequestHandler<TRequest, TResponse>
                where TRequest : IRequest<TResponse> =>
                services.AddHandler(ServiceLifetime.Scoped, addPipelines);

        public static IServiceCollection AddSingleton<THandler, TRequest, TResponse>(
            this IServiceCollection services,
            Action<MediatrPipelineBuilder<THandler, TRequest, TResponse>> addPipelines = null)
                where THandler : class, IRequestHandler<TRequest, TResponse>
                where TRequest : IRequest<TResponse> =>
                services.AddHandler(ServiceLifetime.Singleton, addPipelines);

        public static IServiceCollection AddTransient<THandler, TRequest, TResponse>(
            this IServiceCollection services,
            Action<MediatrPipelineBuilder<THandler, TRequest, TResponse>> addPipelines = null)
                where THandler : class, IRequestHandler<TRequest, TResponse>
                where TRequest : IRequest<TResponse> =>
                services.AddHandler(ServiceLifetime.Transient, addPipelines);
    }

    public class MediatrPipelineBuilder<THandler, TRequest, TResponse>
        where THandler: IRequestHandler<TRequest, TResponse>
        where TRequest: IRequest<TResponse>
    {
        private readonly IServiceCollection services;
        private readonly ServiceLifetime lifetime;

        internal MediatrPipelineBuilder(
            IServiceCollection services,
            ServiceLifetime lifetime)
        {
            this.services = services;
            this.lifetime = lifetime;
        }


        public MediatrPipelineBuilder<THandler, TRequest, TResponse> WithPipeline<TPipeline>()
            where TPipeline: class, IPipelineBehavior<TRequest, TResponse>
        {
            services.Add<IPipelineBehavior<TRequest, TResponse>, TPipeline>(lifetime);
            return this;
        }
    }
}
