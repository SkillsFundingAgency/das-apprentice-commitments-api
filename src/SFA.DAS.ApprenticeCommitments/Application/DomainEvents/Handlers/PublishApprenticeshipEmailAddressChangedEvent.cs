using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.ApprenticeCommitments.Messages.Events;
using SFA.DAS.NServiceBus.Services;

namespace SFA.DAS.ApprenticeCommitments.Application.DomainEvents.Handlers
{
    internal class PublishApprenticeshipEmailAddressChangedEvent : INotificationHandler<ApprenticeEmailAddressChanged>
    {
        private readonly IEventPublisher _eventPublisher;
        private readonly ILogger<PublishApprenticeshipEmailAddressChangedEvent> _logger;

        public PublishApprenticeshipEmailAddressChangedEvent(IEventPublisher eventPublisher, ILogger<PublishApprenticeshipEmailAddressChangedEvent> logger)
        {
            _eventPublisher = eventPublisher;
            _logger = logger;
        }

        public async Task Handle(ApprenticeEmailAddressChanged notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Processing ApprenticeEmailAddressChanged for {apprentice} ", notification.Apprentice.Id);

            foreach (var apprenticeship in notification.Apprentice.Apprenticeships)
            {
                _logger.LogInformation("Processing ApprenticeshipEmailAddressChanged for {apprentice} - {apprenticeship} ", notification.Apprentice.Id, apprenticeship.Id);
                await _eventPublisher.Publish(new ApprenticeshipEmailAddressChangedEvent
                {
                    ApprenticeId = apprenticeship.ApprenticeId,
                    CommitmentsApprenticeshipId = apprenticeship.LatestRevision.CommitmentsApprenticeshipId
                });
            }
        }
    }
}