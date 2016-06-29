using System.Threading;
using System.Threading.Tasks;

namespace Architecture.Core
{
    public interface IHandleEvent<in TEvent>
        where TEvent : IEvent
    {
        Task Handle(
            TEvent message, 
            CancellationToken cancellationToken = default(CancellationToken));
    }
}
