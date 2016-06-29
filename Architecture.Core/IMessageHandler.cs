using System.Threading;
using System.Threading.Tasks;

namespace Architecture.Core
{
    public interface IMessageHandler<in TMessage>
        where TMessage : IMessage
    {
        Task Handle(
            TMessage message, 
            CancellationToken cancellationToken = default(CancellationToken));
    }
}
