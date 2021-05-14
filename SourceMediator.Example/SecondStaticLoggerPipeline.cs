using System.Threading;
using System.Threading.Tasks;

namespace SourceMediator.Example
{
    [PipelineOrder(1)]
    public class SecondStaticLoggerPipeline<TRequest, TResponse> : IPipeline<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        public async Task<TResponse> Execute(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            StaticLogger.Log($"Second logger: Executing request '{typeof(TRequest).Name}'");

            var result = await next();
            
            StaticLogger.Log($"Second logger: Executed request '{typeof(TRequest).Name}' and received response '{typeof(TResponse).Name}'");

            return result;
        }
    }
}