using System.Threading.Tasks;
using FluentAssertions;
using SourceMediator.Example;
using Xunit;

namespace SourceMediator.Test
{
    public class UnitTest1
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
    }
}