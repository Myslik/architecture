﻿using System;
using System.Reflection;

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

        public TResponse Send<TResponse>(IRequest<TResponse> request)
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
            try
            {
                return (TResponse)methodInfo.Invoke(handler, new[] { request });
            }
            catch (TargetInvocationException ex) when (ex.InnerException != null)
            {
                throw ex.InnerException;
            }
        }
    }
}