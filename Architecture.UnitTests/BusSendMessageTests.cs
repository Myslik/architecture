using Architecture.Core;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Architecture.UnitTests
{
    public class BusSendMessageTests
    {
        [Fact]
        public void SendTest()
        {
            var handlerFactory = new SimpleHandlerFactory(
                typeof(IMessageHandler<SimpleMessage>),
                typeof(SimpleMessageHandler));
            var bus = new Bus(handlerFactory);
            bus.Send(new SimpleMessage()).Wait();
        }

        [Fact]
        public void SendWithoutHandlerTest()
        {
            var handlerFactory = new SimpleHandlerFactory();
            var bus = new Bus(handlerFactory);
            bus.Send(new SimpleMessage()).Wait();
        }

        [Fact]
        public void SendWithMultipleHandlersTest()
        {
            var handlerFactory = new SimpleHandlerFactory(
                typeof(IMessageHandler<SimpleMessage>),
                new[] { typeof(SimpleMessageHandler), typeof(AnotherMessageHandler) });
            var bus = new Bus(handlerFactory);
            bus.Send(new SimpleMessage()).Wait();
        }

        [Fact]
        public void SendWithFailingHandlerTest()
        {
            var handlerFactory = new SimpleHandlerFactory(
                typeof(IMessageHandler<SimpleMessage>),
                typeof(FailingMessageHandler));
            var bus = new Bus(handlerFactory);
            var task = bus.Send(new SimpleMessage());
            Assert.Throws<AggregateException>(
                () => task.Wait());
        }

        [Fact]
        public void SendWithMultipleFailingHandlerTest()
        {
            var handlerFactory = new SimpleHandlerFactory(
                typeof(IMessageHandler<SimpleMessage>),
                new[] { typeof(FailingMessageHandler), typeof(AnotherFailingMessageHandler) });
            var bus = new Bus(handlerFactory);
            var task = bus.Send(new SimpleMessage());
            Assert.Throws<AggregateException>(
                () => task.Wait());
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
