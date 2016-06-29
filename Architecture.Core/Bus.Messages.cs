using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Architecture.Core
{
    public partial class Bus
    {
        [DebuggerStepThrough, DebuggerHidden]
        public async Task Send(IMessage message, CancellationToken cancellationToken)
        {
            var messageType = message.GetType();
            var handlers = _handlerFactory.CreateMessageHandlers(messageType);
            foreach (var handler in handlers)
            {
                InjectHandler(handler);
                var wrapper = WrapMessageHandler(messageType, handler);
                await wrapper.Handle(message, cancellationToken);
            }
        }

        private static MessageHandler WrapMessageHandler(Type messageType, object handler)
        {
            var wrapperType = typeof(MessageHandler<>).MakeGenericType(messageType);
            return (MessageHandler)Activator.CreateInstance(wrapperType, handler);
        }

        private abstract class MessageHandler
        {
            public abstract Task Handle(IMessage message, CancellationToken cancellationToken);
        }

        private sealed class MessageHandler<TMessage> : MessageHandler
            where TMessage : IMessage
        {
            private readonly IMessageHandler<TMessage> _inner;

            public MessageHandler(IMessageHandler<TMessage> inner)
            {
                _inner = inner;
            }

            [DebuggerStepThrough, DebuggerHidden]
            public override async Task Handle(IMessage message, CancellationToken cancellationToken)
            {
                await _inner.Handle((TMessage)message, cancellationToken);
            }
        }
    }
}
