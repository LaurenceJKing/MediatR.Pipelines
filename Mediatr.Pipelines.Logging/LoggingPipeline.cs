using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Mediatr.Pipelines.Logging
{
    public class LoggingPipeline<TRequest, TResponse> :
        IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ILogger logger;

        public LoggingPipeline(ILogger logger)
        {
            this.logger = logger;
        }

        public Task<TResponse> Handle(TRequest request,
            CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next)
        {
            try
            {
                return next();
            }
            catch(Exception e)
            {
                logger.LogError(e, "");
                throw;
            }
        }
    }
}
