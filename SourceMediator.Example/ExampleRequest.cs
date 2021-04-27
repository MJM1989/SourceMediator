using System.Threading;
using System.Threading.Tasks;

namespace SourceMediator.Example
{
    public class ExampleRequest : IRequest<ExampleResponse>
    {
        public bool WillSucceed { get; set; }   
    }

    public class ExampleResponse
    {
        public bool Success { get; set; }
    }
    
    public class ExampleRequestHandler : IRequestHandler<ExampleRequest, ExampleResponse>
    {
        public Task<ExampleResponse> Handle(ExampleRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new ExampleResponse {Success = request.WillSucceed});
        }
    }
}