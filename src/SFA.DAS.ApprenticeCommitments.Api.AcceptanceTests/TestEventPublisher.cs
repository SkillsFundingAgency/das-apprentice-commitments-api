using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.NServiceBus.Services;

namespace SFA.DAS.ApprenticeCommitments.Api.AcceptanceTests
{
    public class TestEventPublisher : IMessageSession
    {
        private readonly ConcurrentBag<PublishedEvent> _eventsPublished ;

        public TestEventPublisher(ConcurrentBag<PublishedEvent> eventsPublished)
        {
            _eventsPublished = eventsPublished;
        }

        public Task Send(object message, SendOptions options)
        {
            throw new NotImplementedException();
        }

        public Task Send<T>(Action<T> messageConstructor, SendOptions options)
        {
            throw new NotImplementedException();
        }

        public async Task Publish(object message, PublishOptions options)
        {
            _eventsPublished.Add(new PublishedEvent(message));
            await Task.CompletedTask;
        }

        public Task Publish<T>(Action<T> messageConstructor, PublishOptions publishOptions)
        {
            throw new NotImplementedException();
        }

        public Task Subscribe(Type eventType, SubscribeOptions options)
        {
            throw new NotImplementedException();
        }

        public Task Unsubscribe(Type eventType, UnsubscribeOptions options)
        {
            throw new NotImplementedException();
        }
    }
}