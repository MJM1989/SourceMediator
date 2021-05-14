using System.Threading.Tasks;

namespace SourceMediator
{
    public delegate Task<TResponse> RequestHandlerDelegate<TResponse>();
}