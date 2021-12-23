using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using SFA.DAS.NServiceBus.Services;

namespace SFA.DAS.ApprenticeCommitments.Api.AcceptanceTests
{
    public class TestEventPublisher : IEventPublisher
    {
        private readonly ConcurrentBag<PublishedEvent> _eventsPublished ;

        public TestEventPublisher(ConcurrentBag<PublishedEvent> eventsPublished)
        {
            _eventsPublished = eventsPublished;
        }

        public async Task Publish<T>(T message) where T : class
        {
            _eventsPublished.Add(new PublishedEvent(message));
            await Task.CompletedTask;
        }

        public Task Publish<T>(Func<T> messageFactory) where T : class
        {
            return Publish(messageFactory.Invoke());
        }
    }
}