using Architecture.Core;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Architecture.UnitTests
{
    public class BusSendRequestTests
    {
        [Fact]
        public async void SendTest()
        {
            var handlerFactory = new SimpleHandlerFactory();
            handlerFactory.Register<IHandleRequest<SimpleRequest>, SimpleRequestHandler>();
            var bus = new Bus(handlerFactory);
            await bus.Send(new SimpleRequest(), CancellationToken.None);
        }

        [Fact]
        public async void SendNullTest()
        {
            var handlerFactory = new SimpleHandlerFactory();
            IBus bus = new Bus(handlerFactory);
            SimpleRequest request = null;
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => bus.Send(request));
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
            handlerFactory.Register<IHandleRequest<SimpleRequest>, FailingRequestHandler>();
            var bus = new Bus(handlerFactory);
            await Assert.ThrowsAsync<NotImplementedException>(
                () => bus.Send(new SimpleRequest(), CancellationToken.None));
        }

        [Fact]
        public async void SendWithBadHandlerTest()
        {
            var handlerFactory = new SimpleHandlerFactory();
            handlerFactory.Register<IHandleRequest<SimpleRequest>, BadRequestHandler>();
            var bus = new Bus(handlerFactory);
            await Assert.ThrowsAsync<NotImplementedException>(
                () => bus.Send(new SimpleRequest(), CancellationToken.None));
        }

        private class RequestWithoutHandler : IRequest { }

        private class SimpleRequest : IRequest { }

        private class SimpleRequestHandler :
            IHandleRequest<SimpleRequest>
        {
            public Task Handle(SimpleRequest request, CancellationToken cancellationToken)
            {
                return Task.FromResult(0);
            }
        }

        private class BadRequestHandler : SimpleRequestHandler
        {
            public BadRequestHandler()
            {
                throw new NotImplementedException();
            }
        }

        private class FailingRequestHandler :
            IHandleRequest<SimpleRequest>
        {
            public Task Handle(SimpleRequest request, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }
        }
    }
}
