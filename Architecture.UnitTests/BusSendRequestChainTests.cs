using Architecture.Core;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Architecture.UnitTests
{
    public class BusSendRequestChainTests
    {
        [Fact]
        public async void SendRequestChainTest()
        {
            var handlerFactory = new SimpleHandlerFactory();
            handlerFactory.Register<IHandleRequest<FirstRequest>, FirstRequestHandler>();
            handlerFactory.Register<IHandleRequest<SecondRequest>, SecondRequestHandler>();
            IBus bus = new Bus(handlerFactory);
            await bus.Send(new FirstRequest());
        }

        private class FirstRequest : IRequest { }

        private class FirstRequestHandler : Handler, IHandleRequest<FirstRequest>
        {
            public async Task Handle(FirstRequest message, CancellationToken cancellationToken)
            {
                await Bus.Send(new SecondRequest(), cancellationToken);
            }
        }

        private class SecondRequest : IRequest { }

        private class SecondRequestHandler : Handler, IHandleRequest<SecondRequest>
        {
            public Task Handle(SecondRequest message, CancellationToken cancellationToken = default(CancellationToken))
            {
                return Task.FromResult(0);
            }
        }
    }
}
