using System.Threading;
using System.Threading.Tasks;

namespace SourceMediator.Example
{
    public class SecondExampleRequest : IRequest<SecondExampleResponse>
    {
        public bool WillSucceed { get; set; }   
    }

    public class SecondExampleResponse
    {
        public bool Success { get; set; }
    }
    
    public class SecondExampleRequestHandler : IRequestHandler<SecondExampleRequest, SecondExampleResponse>
    {
        public Task<SecondExampleResponse> Handle(SecondExampleRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new SecondExampleResponse {Success = request.WillSucceed});
        }
    }
}