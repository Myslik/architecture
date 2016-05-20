using System.Threading.Tasks;

namespace Architecture.Core
{
    public interface IBus
    {
        Task Send(IMessage message);
        TResponse Send<TResponse>(IRequest<TResponse> request);
    }
}
