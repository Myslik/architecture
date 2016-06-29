using Architecture.Core;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Architecture.UnitTests
{
    public class BusSendMessageChainTests
    {
        [Fact]
        public async void SendMessageChainTest()
        {
            var handlerFactory = new SimpleHandlerFactory();
            handlerFactory.Register<IMessageHandler<FirstMessage>, FirstMessageHandler>();
            handlerFactory.Register<IMessageHandler<SecondMessage>, SecondMessageHandler>();
            IBus bus = new Bus(handlerFactory);
            await bus.Send(new FirstMessage());
        }

        private class FirstMessage : IMessage { }

        private class FirstMessageHandler : Handler, IMessageHandler<FirstMessage>
        {
            public async Task Handle(FirstMessage message, CancellationToken cancellationToken)
            {
                await Bus.Send(new SecondMessage(), cancellationToken);
            }
        }

        private class SecondMessage : IMessage { }

        private class SecondMessageHandler : Handler, IMessageHandler<SecondMessage>
        {
            public Task Handle(SecondMessage message, CancellationToken cancellationToken = default(CancellationToken))
            {
                return Task.FromResult(0);
            }
        }
    }
}
