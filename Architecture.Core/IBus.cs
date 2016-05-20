using System.Threading.Tasks;

namespace Architecture.Core
{
    public interface IBus
    {
        Task Send(IMessage message);
        Task<TResponse> Send<TResponse>(IRequest<TResponse> request);
    }
}
