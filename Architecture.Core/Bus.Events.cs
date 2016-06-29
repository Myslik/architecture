using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Architecture.Core
{
    public partial class Bus
    {
        [DebuggerStepThrough, DebuggerHidden]
        public virtual async Task Send(IEvent @event, CancellationToken cancellationToken)
        {
            Guard.AgainstNull(nameof(@event), @event);
            var messageType = @event.GetType();
            var handlers = _handlerFactory.CreateEventHandlers(messageType);
            foreach (var handler in handlers)
            {
                InjectHandler(handler);
                var wrapper = WrapEventHandler(messageType, handler);
                await wrapper.Handle(@event, cancellationToken);
            }
        }

        private static EventHandler WrapEventHandler(Type eventType, object handler)
        {
            var wrapperType = typeof(EventHandler<>).MakeGenericType(eventType);
            return (EventHandler)Activator.CreateInstance(wrapperType, handler);
        }

        private abstract class EventHandler
        {
            public abstract Task Handle(IEvent @event, CancellationToken cancellationToken);
        }

        private sealed class EventHandler<TEvent> : EventHandler
            where TEvent : IEvent
        {
            private readonly IHandleEvent<TEvent> _inner;

            public EventHandler(IHandleEvent<TEvent> inner)
            {
                _inner = inner;
            }

            [DebuggerStepThrough, DebuggerHidden]
            public override async Task Handle(IEvent @event, CancellationToken cancellationToken)
            {
                await _inner.Handle((TEvent)@event, cancellationToken);
            }
        }
    }
}
