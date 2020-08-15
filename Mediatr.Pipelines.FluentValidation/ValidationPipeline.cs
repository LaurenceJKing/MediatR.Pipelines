using FluentValidation;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Mediatr.Pipelines.FluentValidation
{
    /// <summary>
    /// A MediatR pipeline that will validate the request.
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    public class ValidationPipeline<TRequest, TResponse> :
        IPipelineBehavior<TRequest, TResponse>
    {
        private readonly IValidator<TRequest> validator;

        public ValidationPipeline(IValidator<TRequest> validator)
        {
            this.validator = validator;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next)
        {
            var result = await validator.ValidateAsync(request);
            if (!result.IsValid) throw new ValidationException(result.Errors);
            return await next();
        }
    }
}
