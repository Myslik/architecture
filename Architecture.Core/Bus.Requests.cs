using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Architecture.Core
{
    public partial class Bus
    {
        [DebuggerStepThrough, DebuggerHidden]
        public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken)
        {
            var requestType = request.GetType();
            var handler = _handlerFactory.CreateRequestHandler<TResponse>(requestType);
            if (handler == null)
            {
                throw new InvalidOperationException(HANDLER_NOT_FOUND + request.GetType());
            }
            InjectHandler(handler);
            var wrapper = WrapRequestHandler<TResponse>(requestType, handler);
            return await wrapper.Handle(request, cancellationToken);
        }

        private static RequestHandler<TResponse> WrapRequestHandler<TResponse>(Type requestType, object handler)
        {
            var wrapperType = typeof(RequestHandler<,>).MakeGenericType(requestType, typeof(TResponse));
            return (RequestHandler<TResponse>)Activator.CreateInstance(wrapperType, handler);
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
    }
}
