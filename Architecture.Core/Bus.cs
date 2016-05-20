using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Architecture.Core
{
    public class Bus : IBus
    {
        private const string HANDLER_NOT_FOUND =
            "Handler was not found for request of type ";
        private readonly IHandlerFactory _handlerFactory;

        public Bus(IHandlerFactory handlerFactory)
        {
            _handlerFactory = handlerFactory;
        }

        public async Task Send(IMessage message)
        {
            var handlerType = typeof(IMessageHandler<>).MakeGenericType(message.GetType());
            object[] handlers = _handlerFactory.CreateMany(handlerType);
            if (handlers != null)
            {
                var tasks = new List<Task>();
                string methodName = nameof(IMessageHandler<IMessage>.Handle);
                MethodInfo methodInfo = handlerType.GetMethod(methodName);
                foreach (var handler in handlers)
                {
                    var task = (Task)methodInfo.Invoke(handler, new[] { message });
                    await task;
                }
            }
        }

        public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request)
        {
            var handlerType = typeof(IRequestHandler<,>).MakeGenericType(request.GetType(), typeof(TResponse));
            object handler;
            try
            {
                handler = _handlerFactory.Create(handlerType);

                if (handler == null)
                {
                    throw new InvalidOperationException(HANDLER_NOT_FOUND + request.GetType());
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(HANDLER_NOT_FOUND + request.GetType(), ex);
            }
            string methodName = nameof(IRequestHandler<IRequest<TResponse>, TResponse>.Handle);
            MethodInfo methodInfo = handlerType.GetMethod(methodName);
            var task = (Task<TResponse>)methodInfo.Invoke(handler, new[] { request });
            return await task;
        }
    }
}
