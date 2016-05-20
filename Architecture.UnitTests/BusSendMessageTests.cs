using Architecture.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            bus.Send(new SimpleMessage());
        }

        [Fact]
        public void SendWithoutHandlerTest()
        {
            var handlerFactory = new SimpleHandlerFactory();
            var bus = new Bus(handlerFactory);
            bus.Send(new SimpleMessage());
        }

        [Fact]
        public void SendWithMultipleHandlersTest()
        {
            var handlerFactory = new SimpleHandlerFactory(
                typeof(IMessageHandler<SimpleMessage>),
                new[] { typeof(SimpleMessageHandler), typeof(AnotherMessageHandler) });
            var bus = new Bus(handlerFactory);
            bus.Send(new SimpleMessage());
        }

        [Fact]
        public void SendWithFailingHandlerTest()
        {
            var handlerFactory = new SimpleHandlerFactory(
                typeof(IMessageHandler<SimpleMessage>),
                typeof(FailingMessageHandler));
            var bus = new Bus(handlerFactory);
            Assert.Throws<NotImplementedException>(
                () => bus.Send(new SimpleMessage()));
        }

        [Fact]
        public void SendWithMultipleFailingHandlerTest()
        {
            var handlerFactory = new SimpleHandlerFactory(
                typeof(IMessageHandler<SimpleMessage>),
                new[] { typeof(FailingMessageHandler), typeof(AnotherFailingMessageHandler) });
            var bus = new Bus(handlerFactory);
            Assert.Throws<AggregateException>(
                () => bus.Send(new SimpleMessage()));
        }

        private class SimpleMessage : IMessage { }

        private class SimpleMessageHandler : IMessageHandler<SimpleMessage>
        {
            public void Handle(SimpleMessage message)
            {

            }
        }

        private class AnotherMessageHandler : IMessageHandler<SimpleMessage>
        {
            public void Handle(SimpleMessage message)
            {

            }
        }

        private class FailingMessageHandler: IMessageHandler<SimpleMessage>
        {
            public void Handle(SimpleMessage message)
            {
                throw new NotImplementedException();
            }
        }

        private class AnotherFailingMessageHandler : IMessageHandler<SimpleMessage>
        {
            public void Handle(SimpleMessage message)
            {
                throw new NotImplementedException();
            }
        }
    }
}
