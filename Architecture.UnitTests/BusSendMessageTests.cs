using Architecture.Core;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Architecture.UnitTests
{
    public class BusSendMessageTests
    {
        [Fact]
        public async void SendTest()
        {
            var handlerFactory = new SimpleHandlerFactory(
                typeof(IMessageHandler<SimpleMessage>),
                typeof(SimpleMessageHandler));
            var bus = new Bus(handlerFactory);
            await bus.Send(new SimpleMessage());
        }

        [Fact]
        public async void SendWithoutHandlerTest()
        {
            var handlerFactory = new SimpleHandlerFactory();
            var bus = new Bus(handlerFactory);
            await bus.Send(new SimpleMessage());
        }

        [Fact]
        public async void SendWithMultipleHandlersTest()
        {
            var handlerFactory = new SimpleHandlerFactory(
                typeof(IMessageHandler<SimpleMessage>),
                new[] { typeof(SimpleMessageHandler), typeof(AnotherMessageHandler) });
            var bus = new Bus(handlerFactory);
            await bus.Send(new SimpleMessage());
        }

        [Fact]
        public async void SendWithFailingHandlerTest()
        {
            var handlerFactory = new SimpleHandlerFactory(
                typeof(IMessageHandler<SimpleMessage>),
                typeof(FailingMessageHandler));
            var bus = new Bus(handlerFactory);
            await Assert.ThrowsAsync<NotImplementedException>(
                () => bus.Send(new SimpleMessage()));
        }

        [Fact]
        public async void SendWithMultipleFailingHandlerTest()
        {
            var handlerFactory = new SimpleHandlerFactory(
                typeof(IMessageHandler<SimpleMessage>),
                new[] { typeof(FailingMessageHandler), typeof(AnotherFailingMessageHandler) });
            var bus = new Bus(handlerFactory);
            await Assert.ThrowsAsync<NotImplementedException>(
                () => bus.Send(new SimpleMessage()));
        }

        private class SimpleMessage : IMessage { }

        private class SimpleMessageHandler : IMessageHandler<SimpleMessage>
        {
            public Task Handle(SimpleMessage message)
            {
                return Task.FromResult(0);
            }
        }

        private class AnotherMessageHandler : IMessageHandler<SimpleMessage>
        {
            public Task Handle(SimpleMessage message)
            {
                return Task.FromResult(0);
            }
        }

        private class FailingMessageHandler: IMessageHandler<SimpleMessage>
        {
            public Task Handle(SimpleMessage message)
            {
                throw new NotImplementedException();
            }
        }

        private class AnotherFailingMessageHandler : IMessageHandler<SimpleMessage>
        {
            public Task Handle(SimpleMessage message)
            {
                throw new NotImplementedException();
            }
        }
    }
}
