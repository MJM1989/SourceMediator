using System.Threading.Tasks;
using FluentAssertions;
using SourceMediator.Example;
using Xunit;

namespace SourceMediator.Test
{
    public class MediatorTests
    {
        [Fact]
        public async Task StartWithTwoExamples()
        {
            var mediator = new Mediator();

            var response = await mediator.Send(new ExampleRequest {WillSucceed = true});
            var response2 = await mediator.Send(new SecondExampleRequest {WillSucceed = false});

            response.Success.Should().BeTrue();
            response2.Success.Should().BeFalse();
        }

        [Fact]
        public async Task TestSimplePipelineBehavior()
        {
            var mediator = new Mediator();

            var response = await mediator.Send(new ExampleRequest {WillSucceed = true});

            StaticLogger.LogMessages.Should().Contain("Executing request 'ExampleRequest'");
            StaticLogger.LogMessages.Should().Contain("Executed request 'ExampleRequest' and received response 'ExampleResponse'");
        }
    }
}