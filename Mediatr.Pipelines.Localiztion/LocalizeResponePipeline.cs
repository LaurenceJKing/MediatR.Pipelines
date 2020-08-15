using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Mediatr.Pipelines.Localiztion
{
    /// <summary>
    /// A MediatR pipeline that injects localized strings into the response.
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    public class LocalizeResponsePipeline<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly ILocalizationResolver localizationResolver;

        public LocalizeResponsePipeline(ILocalizationResolver localizationResolver)
        {
            this.localizationResolver = localizationResolver;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next)
        {
            var response = await next();
            localizationResolver.Localize(response);
            return response;
        }
    }


    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class LocalizeAttribute:Attribute
    {
        public string Key { get; set; }
    }
}
