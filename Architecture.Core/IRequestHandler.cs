using System.Threading;
using System.Threading.Tasks;

namespace Architecture.Core
{
    public interface IRequestHandler<in TRequest>
        where TRequest : IRequest
    {
        Task Handle(
            TRequest request,
            CancellationToken cancellationToken = default(CancellationToken));
    }

    public interface IRequestHandler<in TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        Task<TResponse> Handle(
            TRequest request, 
            CancellationToken cancellationToken = default(CancellationToken));
    }
}
