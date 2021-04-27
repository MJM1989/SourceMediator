using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace SourceMediator.Generator
{
    [Generator]
    public class MediatorGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            // Do not use at the mo
        }

        public void Execute(GeneratorExecutionContext context)
        {
            var sourceBuilder = new StringBuilder(@"
using System.Threading;
using System.Threading.Tasks;

namespace SourceMediator.Example
{
    public class Mediator
    {
        private readonly IRequestHandler<ExampleRequest, ExampleResponse> _exampleRequestHandler
            = new ExampleRequestHandler();
        private readonly IRequestHandler<SecondExampleRequest, SecondExampleResponse> _secondExampleRequestHandler
            = new SecondExampleRequestHandler();

        public async Task<ExampleResponse> Send(ExampleRequest request,
            CancellationToken cancellationToken = default)
        {
            return await _exampleRequestHandler.Handle(request, cancellationToken);
        }

        public async Task<SecondExampleResponse> Send(SecondExampleRequest request,
            CancellationToken cancellationToken = default)
        {
            return await _secondExampleRequestHandler.Handle(request, cancellationToken);
        }
    }
}
");
            context.AddSource("SourceMediator", SourceText.From(sourceBuilder.ToString(), Encoding.UTF8));
        }
    }
}