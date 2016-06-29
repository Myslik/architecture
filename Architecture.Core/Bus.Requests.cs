using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Architecture.Core
{
    public partial class Bus
    {
        [DebuggerStepThrough, DebuggerHidden]
        public virtual async Task Send(IRequest request, CancellationToken cancellationToken)
        {
            var requestType = request.GetType();
            var handler = _handlerFactory.CreateRequestHandler(requestType);
            if (handler == null)
            {
                throw new InvalidOperationException(HANDLER_NOT_FOUND + request.GetType());
            }
            InjectHandler(handler);
            var wrapper = WrapRequestHandler(requestType, handler);
            await wrapper.Handle(request, cancellationToken);
        }

        [DebuggerStepThrough, DebuggerHidden]
        public virtual async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken)
        {
            var requestType = request.GetType();
            var handler = _handlerFactory.CreateRequestHandler<TResponse>(requestType);
            if (handler == null)
            {
                throw new InvalidOperationException(HANDLER_NOT_FOUND + request.GetType());
            }
            InjectHandler(handler);
            var wrapper = WrapRequestHandlerWithResponse<TResponse>(requestType, handler);
            return await wrapper.Handle(request, cancellationToken);
        }

        private static RequestHandler WrapRequestHandler(Type requestType, object handler)
        {
            var wrapperType = typeof(RequestHandler<>).MakeGenericType(requestType);
            return (RequestHandler)Activator.CreateInstance(wrapperType, handler);
        }

        private abstract class RequestHandler
        {
            public abstract Task Handle(IRequest request, CancellationToken cancellationToken);
        }

        private sealed class RequestHandler<TRequest> : RequestHandler
            where TRequest : IRequest
        {
            private readonly IHandleRequest<TRequest> _inner;

            public RequestHandler(IHandleRequest<TRequest> inner)
            {
                _inner = inner;
            }

            [DebuggerStepThrough, DebuggerHidden]
            public override async Task Handle(IRequest request, CancellationToken cancellationToken)
            {
                await _inner.Handle((TRequest)request, cancellationToken);
            }
        } 

        private static RequestHandlerWithResponse<TResponse> WrapRequestHandlerWithResponse<TResponse>(Type requestType, object handler)
        {
            var wrapperType = typeof(RequestHandlerWithResponse<,>).MakeGenericType(requestType, typeof(TResponse));
            return (RequestHandlerWithResponse<TResponse>)Activator.CreateInstance(wrapperType, handler);
        }

        private abstract class RequestHandlerWithResponse<TResponse>
        {
            public abstract Task<TResponse> Handle(IRequest<TResponse> request, CancellationToken cancellationToken);
        }

        private sealed class RequestHandlerWithResponse<TRequest, TResponse> : RequestHandlerWithResponse<TResponse>
            where TRequest : IRequest<TResponse>
        {
            private readonly IHandleRequest<TRequest, TResponse> _inner;

            public RequestHandlerWithResponse(IHandleRequest<TRequest, TResponse> inner)
            {
                _inner = inner;
            }

            [DebuggerStepThrough, DebuggerHidden]
            public override async Task<TResponse> Handle(IRequest<TResponse> request, CancellationToken cancellationToken)
            {
                return await _inner.Handle((TRequest)request, cancellationToken);
            }
        }
    }
}
