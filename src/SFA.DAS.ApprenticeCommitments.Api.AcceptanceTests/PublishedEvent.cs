namespace SFA.DAS.ApprenticeCommitments.Api.AcceptanceTests
{
    public class PublishedEvent
    {
        public object Event { get; }
        public PublishedEvent(object @event)
        {
            Event = @event;
        }
    }
}