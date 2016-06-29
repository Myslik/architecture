using System.Threading;
using System.Threading.Tasks;

namespace Architecture.Core
{
    public interface IBus
    {
        Task Send(
            IEvent @event, 
            CancellationToken cancellationToken = default(CancellationToken));

        Task Send(
            IRequest request,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<TResponse> Send<TResponse>(
            IRequest<TResponse> request,
            CancellationToken cancellationToken = default(CancellationToken));
    }
}
