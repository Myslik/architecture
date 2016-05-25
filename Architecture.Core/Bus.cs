using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Architecture.Core
{
    public class Bus : IBus
    {
        private const string HANDLER_NOT_FOUND =
            "Handler was not found for request of type ";
        private readonly HandlerFactory _handlerFactory;

        public Bus(IHandlerFactory handlerFactory)
        {
            _handlerFactory = new HandlerFactory(handlerFactory);
        }

        public async Task Send(IMessage message, CancellationToken cancellationToken)
        {
            var messageType = message.GetType();
            var handlers = _handlerFactory.CreateMessageHandlers(messageType);
            foreach(var handler in handlers)
            {
                var wrapper = WrapMessageHandler(messageType, handler);
                await wrapper.Handle(message, cancellationToken);
            }
        }

        public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken)
        {
            var requestType = request.GetType();
            var handler = _handlerFactory.CreateRequestHandler<TResponse>(requestType);
            if (handler == null)
            {
                throw new InvalidOperationException(HANDLER_NOT_FOUND + request.GetType());
            }
            var wrapper = WrapRequestHandler<TResponse>(requestType, handler);
            return await wrapper.Handle(request, cancellationToken);
        }

        private MessageHandler WrapMessageHandler(Type messageType, object handler)
        {
            var wrapperType = typeof(MessageHandler<>).MakeGenericType(messageType);
            return (MessageHandler)Activator.CreateInstance(wrapperType, handler);
        }

        private RequestHandler<TResponse> WrapRequestHandler<TResponse>(Type requestType, object handler)
        {
            var wrapperType = typeof(RequestHandler<,>).MakeGenericType(requestType, typeof(TResponse));
            return (RequestHandler<TResponse>)Activator.CreateInstance(wrapperType, handler);
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

        private abstract class RequestHandler<TResponse>
        {
            public abstract Task<TResponse> Handle(IRequest<TResponse> request, CancellationToken cancellationToken);
        }

        private sealed class RequestHandler<TRequest, TResponse> : RequestHandler<TResponse>
            where TRequest : IRequest<TResponse>
        {
            private readonly IRequestHandler<TRequest, TResponse> _inner;

            public RequestHandler(IRequestHandler<TRequest, TResponse> inner)
            {
                _inner = inner;
            }

            [DebuggerStepThrough, DebuggerHidden]
            public override async Task<TResponse> Handle(IRequest<TResponse> request, CancellationToken cancellationToken)
            {
                return await _inner.Handle((TRequest)request, cancellationToken);
            }
        }

        private sealed class HandlerFactory
        {
            private readonly IHandlerFactory _handlerFactory;

            public HandlerFactory(IHandlerFactory handlerFactory)
            {
                _handlerFactory = handlerFactory;
            }

            [DebuggerStepThrough, DebuggerHidden]
            public object[] CreateMessageHandlers(Type messageType)
            {
                var serviceType = typeof(IMessageHandler<>).MakeGenericType(messageType);
                return _handlerFactory.CreateMany(serviceType);
            }

            [DebuggerStepThrough, DebuggerHidden]
            public object CreateRequestHandler<TResponse>(Type requestType)
            {
                var serviceType = typeof(IRequestHandler<,>).MakeGenericType(requestType, typeof(TResponse));
                return _handlerFactory.Create(serviceType);
            }
        }
    }
}
