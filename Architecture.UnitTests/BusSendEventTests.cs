using Architecture.Core;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Architecture.UnitTests
{
    public class BusSendEventTests
    {
        [Fact]
        public async void SendTest()
        {
            var handlerFactory = new SimpleHandlerFactory();
            handlerFactory.Register<IHandleEvent<SimpleEvent>, SimpleEventHandler>();
            var bus = new Bus(handlerFactory);
            await bus.Send(new SimpleEvent(), CancellationToken.None);
        }

        [Fact]
        public async void SendNullTest()
        {
            var handlerFactory = new SimpleHandlerFactory();
            IBus bus = new Bus(handlerFactory);
            SimpleEvent @event = null;
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => bus.Send(@event));
        }

        [Fact]
        public async void SendWithoutHandlerTest()
        {
            var handlerFactory = new SimpleHandlerFactory();
            var bus = new Bus(handlerFactory);
            await bus.Send(new SimpleEvent(), CancellationToken.None);
        }

        [Fact]
        public async void SendWithMultipleHandlersTest()
        {
            var handlerFactory = new SimpleHandlerFactory();
            handlerFactory.Register<IHandleEvent<SimpleEvent>>(typeof(SimpleEventHandler), typeof(AnotherEventHandler));
            var bus = new Bus(handlerFactory);
            await bus.Send(new SimpleEvent(), CancellationToken.None);
        }

        [Fact]
        public async void SendWithFailingHandlerTest()
        {
            var handlerFactory = new SimpleHandlerFactory();
            handlerFactory.Register<IHandleEvent<SimpleEvent>, FailingEventHandler>();
            var bus = new Bus(handlerFactory);
            await Assert.ThrowsAsync<NotImplementedException>(
                () => bus.Send(new SimpleEvent(), CancellationToken.None));
        }

        [Fact]
        public async void SendWithMultipleFailingHandlerTest()
        {
            var handlerFactory = new SimpleHandlerFactory();
            handlerFactory.Register<IHandleEvent<SimpleEvent>>(typeof(FailingEventHandler), typeof(AnotherFailingEventHandler));
            var bus = new Bus(handlerFactory);
            await Assert.ThrowsAsync<NotImplementedException>(
                () => bus.Send(new SimpleEvent(), CancellationToken.None));
        }

        private class SimpleEvent : IEvent { }

        private class SimpleEventHandler : IHandleEvent<SimpleEvent>
        {
            public Task Handle(SimpleEvent message, CancellationToken cancellationToken)
            {
                return Task.FromResult(0);
            }
        }

        private class AnotherEventHandler : IHandleEvent<SimpleEvent>
        {
            public Task Handle(SimpleEvent message, CancellationToken cancellationToken)
            {
                return Task.FromResult(0);
            }
        }

        private class FailingEventHandler: IHandleEvent<SimpleEvent>
        {
            public Task Handle(SimpleEvent message, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }
        }

        private class AnotherFailingEventHandler : IHandleEvent<SimpleEvent>
        {
            public Task Handle(SimpleEvent message, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }
        }
    }
}
