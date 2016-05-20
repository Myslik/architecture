using Architecture.Core;
using System;
using Xunit;

namespace Architecture.UnitTests
{
    public class BusTests
    {
        [Fact]
        public void SendTest()
        {
            var handlerFactory = new SimpleHandlerFactory(
                typeof(IRequestHandler<SimpleRequest, SimpleResponse>),
                typeof(SimpleRequestHandler));
            var bus = new Bus(handlerFactory);
            var response = bus.Send(new SimpleRequest());
            Assert.NotNull(response);
            Assert.IsType<SimpleResponse>(response);
        }

        [Fact]
        public void SendWithoutHandlerTest()
        {
            var handlerFactory = new SimpleHandlerFactory();
            var bus = new Bus(handlerFactory);
            Assert.Throws<InvalidOperationException>(
                () => bus.Send(new RequestWithoutHandler()));
        }

        [Fact]
        public void SendWithFailingHandlerTest()
        {
            var handlerFactory = new SimpleHandlerFactory(
                typeof(IRequestHandler<SimpleRequest, SimpleResponse>),
                typeof(FailingRequestHandler));
            var bus = new Bus(handlerFactory);
            Assert.Throws<NotImplementedException>(
                () => bus.Send(new SimpleRequest()));
        }

        [Fact]
        public void SendWithBadHandlerTest()
        {
            var handlerFactory = new SimpleHandlerFactory(
                typeof(IRequestHandler<SimpleRequest, SimpleResponse>),
                typeof(BadRequestHandler));
            var bus = new Bus(handlerFactory);
            Assert.Throws<InvalidOperationException>(
                () => bus.Send(new SimpleRequest()));
        }

        private class RequestWithoutHandler : IRequest<SimpleResponse> { }

        private class SimpleResponse { }

        private class SimpleRequest : IRequest<SimpleResponse> { }

        private class SimpleRequestHandler : 
            IRequestHandler<SimpleRequest, SimpleResponse>
        {
            public SimpleResponse Handle(SimpleRequest request)
            {
                return new SimpleResponse();
            }
        }

        private class BadRequestHandler : SimpleRequestHandler
        {
            public BadRequestHandler()
            {
                throw new NotImplementedException();
            }
        }

        private class FailingRequestHandler:
            IRequestHandler<SimpleRequest, SimpleResponse>
        {
            public SimpleResponse Handle(SimpleRequest request)
            {
                throw new NotImplementedException();
            }
        }

        private class SimpleHandlerFactory : IHandlerFactory
        {
            private readonly Type _service;
            private readonly Type _impl;

            public SimpleHandlerFactory() { }

            public SimpleHandlerFactory(Type service, Type impl)
            {
                _service = service;
                _impl = impl;
            }

            public object Create(Type serviceType)
            {
                if (serviceType == _service)
                {
                    return Activator.CreateInstance(_impl);
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
