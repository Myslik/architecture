using Architecture.Core;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Architecture.UnitTests
{
    public class BusSendRequestWithResponseTests
    {
        [Fact]
        public async void SendTest()
        {
            var handlerFactory = new SimpleHandlerFactory();
            handlerFactory.Register<IHandleRequest<SimpleRequest, SimpleResponse>, SimpleRequestHandler>();
            var bus = new Bus(handlerFactory);
            var response = await bus.Send(new SimpleRequest(), CancellationToken.None);
            Assert.NotNull(response);
            Assert.IsType<SimpleResponse>(response);
        }

        [Fact]
        public async void SendWithoutHandlerTest()
        {
            var handlerFactory = new SimpleHandlerFactory();
            var bus = new Bus(handlerFactory);
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => bus.Send(new RequestWithoutHandler(), CancellationToken.None));
        }

        [Fact]
        public async void SendWithFailingHandlerTest()
        {
            var handlerFactory = new SimpleHandlerFactory();
            handlerFactory.Register<IHandleRequest<SimpleRequest, SimpleResponse>, FailingRequestHandler>();
            var bus = new Bus(handlerFactory);
            await Assert.ThrowsAsync<NotImplementedException>(
                () => bus.Send(new SimpleRequest(), CancellationToken.None));
        }

        [Fact]
        public async void SendWithBadHandlerTest()
        {
            var handlerFactory = new SimpleHandlerFactory();
            handlerFactory.Register<IHandleRequest<SimpleRequest, SimpleResponse>, BadRequestHandler>();
            var bus = new Bus(handlerFactory);
            await Assert.ThrowsAsync<NotImplementedException>(
                () => bus.Send(new SimpleRequest(), CancellationToken.None));
        }

        private class RequestWithoutHandler : IRequest<SimpleResponse> { }

        private class SimpleResponse { }

        private class SimpleRequest : IRequest<SimpleResponse> { }

        private class SimpleRequestHandler : 
            IHandleRequest<SimpleRequest, SimpleResponse>
        {
            public Task<SimpleResponse> Handle(SimpleRequest request, CancellationToken cancellationToken)
            {
                return Task.FromResult(new SimpleResponse());
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
            IHandleRequest<SimpleRequest, SimpleResponse>
        {
            public Task<SimpleResponse> Handle(SimpleRequest request, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }
        }
    }
}
