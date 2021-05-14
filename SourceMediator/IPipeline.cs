using System.Threading;
using System.Threading.Tasks;

namespace SourceMediator
{
    public interface IPipeline<TRequest, TResponse>  
        where TRequest : IRequest<TResponse>
    {
        Task<TResponse> Execute(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next);
    }
}