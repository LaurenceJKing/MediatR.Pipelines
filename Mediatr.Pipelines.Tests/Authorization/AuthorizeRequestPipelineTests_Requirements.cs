using AutoFixture.Xunit2;
using FluentAssertions;
using Mediatr.Pipelines.Authorisation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace Mediatr.Pipelines.Tests.Authorization
{
    public class AuthorizeRequestPipelineTests_Requirements
    {
        private Task<Response> Next() => Task.FromResult(new Response());

        [Theory]
        [AutoData]
        public async Task Given_request_meets_requirements_then_request_succeeds(
            Mock<IAuthorizationService> authorizationService,
            Mock<IHttpContextAccessor> httpContextAccessor,
            DummyRequirement requirement,
            Request request)
        {
            authorizationService.Setup(a => a.AuthorizeAsync(
                It.IsAny<ClaimsPrincipal>(),
                It.IsAny<object>(),
                It.IsAny<IEnumerable<IAuthorizationRequirement>>()))
                .ReturnsAsync(AuthorizationResult.Success);

            var pipeline = new AuthorizationPipeline<Request, Response>(
                authorizationService.Object,
                httpContextAccessor.Object,
                requirement);

            Func<Task<Response>> test = () => pipeline.Handle(request, default, Next);

            await test.Should().NotThrowAsync<AuthorizationException>();
        }

        [Theory]
        [AutoData]
        public async Task Given_request_does_not_meet_policy_then_request_fails(
           Mock<IAuthorizationService> authorizationService,
           Mock<IHttpContextAccessor> httpContextAccessor,
           DummyRequirement requirement,
           Request request)
        {
            authorizationService.Setup(a => a.AuthorizeAsync(
                It.IsAny<ClaimsPrincipal>(),
                It.IsAny<object>(),
                It.IsAny<IEnumerable<IAuthorizationRequirement>>()))
                .ReturnsAsync(AuthorizationResult.Failed());

            var pipeline = new AuthorizationPipeline<Request, Response>(
                authorizationService.Object,
                httpContextAccessor.Object,
                requirement);

            Func<Task<Response>> test = () => pipeline.Handle(request, default, Next);

            await test.Should().ThrowAsync<AuthorizationException>();
        }

        public class DummyRequirement : IAuthorizationRequirement { }
    }
}
