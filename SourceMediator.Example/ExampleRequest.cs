using System.Threading;
using System.Threading.Tasks;

namespace SourceMediator.Example
{
    /// <summary>
    /// Represents a first example request
    /// </summary>
    public class ExampleRequest : IRequest<ExampleResponse>
    {
        public bool WillSucceed { get; set; }   
    }

    /// <summary>
    /// Represents a first example response
    /// </summary>
    public class ExampleResponse
    {
        public bool Success { get; set; }
    }
    
    public class ExampleRequestHandler : IRequestHandler<ExampleRequest, ExampleResponse>
    {
        /// <summary>
        /// Represents an example request handler. This comment should be copied into to the generated method
        /// </summary>
        public Task<ExampleResponse> Handle(ExampleRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new ExampleResponse {Success = request.WillSucceed});
        }
    }
}