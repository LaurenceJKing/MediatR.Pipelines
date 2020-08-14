using AutoFixture.Xunit2;
using FluentAssertions;
using Mediatr.Pipelines.Logging;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Mediatr.Pipelines.Tests.Logging
{
    public class LoggingPipelineTests
    {
        [Theory]
        [AutoData]
        public async Task Pipeline_logs_exception_when_thrown(
            Mock<ILogger> logger,
            Request request)
        {
            Task<Response> next() => throw new Exception();
            var pipeline = new LoggingPipeline<Request, Response>(logger.Object);

            Func<Task<Response>> test = () => pipeline.Handle(request, default, next);

            await test.Should().ThrowAsync<Exception>();
            logger.Verify(Log, Times.Once);
        }

        [Theory]
        [AutoData]
        public async Task Pipeline_does_not_log_when_no_exception_thrown(
           Mock<ILogger> logger,
           Request request)
        {
            Task<Response> next() => Task.FromResult(new Response());
            var pipeline = new LoggingPipeline<Request, Response>(logger.Object);

            Func<Task<Response>> test = () => pipeline.Handle(request, default, next);

            await test.Should().NotThrowAsync<Exception>();
            logger.Verify(Log, Times.Never);                        
        }

        private Expression<Action<ILogger>> Log =>
            l => l.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((_, __) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((_, __) => true));
    }
}
