using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Architecture.Core
{
    public partial class Bus : IBus
    {
        private const string HANDLER_NOT_FOUND =
            "Handler was not found for request of type ";
        private readonly HandlerFactory _handlerFactory;

        public Bus(IHandlerFactory handlerFactory)
        {
            _handlerFactory = new HandlerFactory(handlerFactory);
        }

        private void InjectHandler(object handler)
        {
            if (typeof(Handler).IsAssignableFrom(handler.GetType()))
            {
                InjectHandler((Handler)handler);
            }
        }

        private void InjectHandler(Handler handler)
        {
            handler.Bus = this;
        }

        private sealed class HandlerFactory
        {
            private readonly IHandlerFactory _handlerFactory;

            public HandlerFactory(IHandlerFactory handlerFactory)
            {
                _handlerFactory = handlerFactory;
            }

            [DebuggerStepThrough, DebuggerHidden]
            public IEnumerable<object> CreateEventHandlers(Type eventType)
            {
                var serviceType = typeof(IHandleEvent<>).MakeGenericType(eventType);
                return _handlerFactory.CreateMany(serviceType);
            }

            [DebuggerStepThrough, DebuggerHidden]
            public object CreateRequestHandler(Type requestType)
            {
                var serviceType = typeof(IHandleRequest<>).MakeGenericType(requestType);
                return _handlerFactory.Create(serviceType);
            }

            [DebuggerStepThrough, DebuggerHidden]
            public object CreateRequestHandler<TResponse>(Type requestType)
            {
                var serviceType = typeof(IHandleRequest<,>).MakeGenericType(requestType, typeof(TResponse));
                return _handlerFactory.Create(serviceType);
            }
        }
    }
}
