using AutoFixture.Xunit2;
using FluentValidation;
using FluentValidation.Results;
using Mediatr.Pipelines.FluentValidation;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;

namespace Mediatr.Pipelines.Tests.Validation
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
            NextInvoked.Should().Be(0);
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
            NextInvoked.Should().Be(1);
        }

        private Task<Response> Next() {
            NextInvoked++;
            return Task.FromResult(new Response());
            }

        private int NextInvoked { get; set; } 
    }
}
