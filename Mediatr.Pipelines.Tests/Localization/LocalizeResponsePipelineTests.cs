using AutoFixture.Xunit2;
using Mediatr.Pipelines.Localiztion;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace Mediatr.Pipelines.Tests.Localization
{
    public class LocalizeResponsePipelineTests
    {
        [Theory]
        [AutoData]
        public async Task Response_should_be_localized(
            Mock<ILocalizationResolver> resolver,
            Request request)
        {
            Task<Response> next() => Task.FromResult(new Response());

            var pipeline = new LocalizeResponsePipeline<Request, Response>(
                resolver.Object);

            await pipeline.Handle(request, default, next);

            resolver.Verify(r => r.Localize(It.IsAny<object>()), Times.Once);

        }

    }
}
