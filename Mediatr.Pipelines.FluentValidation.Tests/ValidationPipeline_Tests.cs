using System;
using Xunit;
using AutoFixture.Xunit2;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using System.Threading;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;

namespace Mediatr.Pipelines.FluentValidation.Tests
{
    public class ValidationPipelineTests
    {
        [Theory]
        [AutoData]
        public async Task Pipeline_should_fail_if_request_is_not_valid(
            Request request,
            Mock<IValidator<Request>> validator)
        {
            validator.Setup(v => v.ValidateAsync(request, default))
                .ReturnsAsync(
                new ValidationResult(
                    new List<ValidationFailure>() {
                        new ValidationFailure("", "")
                    }));

            var handler = new ValidationPipeline<Request, Response>(validator.Object);

            Func<Task<Response>> test = () =>
                handler.Handle(request, default, Next);

            await test.Should().ThrowAsync<ValidationException>();
        }

        [Theory]
        [AutoData]
        public async Task Pipeline_should_call_next_if_request_is_valid(
            Request request,
            Mock<IValidator<Request>> validator)
        {
            validator.Setup(v => v.ValidateAsync(request, default))
                .ReturnsAsync(new ValidationResult());

            var handler = new ValidationPipeline<Request, Response>(validator.Object);

            Func<Task<Response>> test = () =>
                handler.Handle(request, default, Next);

            await test.Should().NotThrowAsync<ValidationException>();
        }

        private Task<Response> Next() => Task.FromResult(new Response());
    }
}
