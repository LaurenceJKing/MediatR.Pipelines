using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Mediatr.Pipelines.Authorisation
{
    public class AuthorizationPipeline<TRequest, TResponse> :
        IPipelineBehavior<TRequest, TResponse>
    {
        private readonly IAuthorizationService authorizationService;
        private readonly IHttpContextAccessor httpContextAccessor;

        private readonly string policyName;
        private readonly AuthorizationPolicy policy;
        private readonly IEnumerable<IAuthorizationRequirement> requirements;

        private AuthorizationPipeline(
            IAuthorizationService authorizationService,
            IHttpContextAccessor httpContextAccessor)
        {
            this.authorizationService = authorizationService;
            this.httpContextAccessor = httpContextAccessor;
        }

        public AuthorizationPipeline(
            IAuthorizationService authorizationService,
            IHttpContextAccessor httpContextAccessor,
            string policyName): 
            this(authorizationService, httpContextAccessor)
        {
            this.policyName = policyName;
        }

        public AuthorizationPipeline(
           IAuthorizationService authorizationService,
           IHttpContextAccessor httpContextAccessor,
           AuthorizationPolicy policy) :
           this(authorizationService, httpContextAccessor)
        {
            this.policy = policy;
        }

        public AuthorizationPipeline(
           IAuthorizationService authorizationService,
           IHttpContextAccessor httpContextAccessor,
           IEnumerable<IAuthorizationRequirement> requirements) :
           this(authorizationService, httpContextAccessor)
        {
            this.requirements = requirements;
        }

        public AuthorizationPipeline(
           IAuthorizationService authorizationService,
           IHttpContextAccessor httpContextAccessor,
           IAuthorizationRequirement requirement) :
           this(authorizationService, httpContextAccessor)
        {
            this.requirements = new IAuthorizationRequirement[] { requirement };
        }

        public async Task<TResponse> Handle(
            TRequest request, 
            CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var response = await next();
            var result = await Authorize(response);

            if (!result.Succeeded)
            {
                throw new AuthorizationException(
                    result.Failure,
                    AuthorizationFailedMessage());
            }

            return response;
        }

        private string AuthorizationFailedMessage()
        {
            var userName = httpContextAccessor.HttpContext?.User?.Identity?.Name;

            userName = string.IsNullOrEmpty(userName) ? "" : $"({userName})";

            return $"The current user {userName} is not authorised to view this resource.";        
        }

        private Task<AuthorizationResult> Authorize(TResponse response)
        {
            var user = httpContextAccessor.HttpContext.User;

            if (requirements != null)
                return authorizationService.AuthorizeAsync(user, response, requirements);
            else if (policy != null)
                return authorizationService.AuthorizeAsync(user, response, policy);
            else if (!string.IsNullOrWhiteSpace(policyName))
                return authorizationService.AuthorizeAsync(user, response, policyName);

            return Task.FromResult(AuthorizationResult.Success());
        }
    }

    [Serializable]
    public class AuthorizationException: Exception
    {
        public AuthorizationException()
        {
        }

        public IEnumerable<string> FailedRequirements { get; } = new List<string>();

        public AuthorizationException(string message) : base(message)
        {
        }

        public AuthorizationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public AuthorizationException(AuthorizationFailure failure, string message) : base(message)
        {
            FailedRequirements = failure.FailedRequirements.Select(r => r.GetType().Name);
        }

        public AuthorizationException(AuthorizationFailure failure, string message, Exception innerException) : base(message, innerException)
        {
            FailedRequirements = failure.FailedRequirements.Select(r => r.GetType().Name);
        }
    }
}
