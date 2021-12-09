using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.ApprenticeCommitments.Messages.Events;
using SFA.DAS.NServiceBus.Services;

namespace SFA.DAS.ApprenticeCommitments.Application.DomainEvents.Handlers
{
    internal class PublishApprenticeshipChangedEvent : INotificationHandler<ApprenticeshipChanged>
    {
        private readonly IEventPublisher _eventPublisher;
        private readonly ILogger<PublishApprenticeshipChangedEvent> _logger;

        public PublishApprenticeshipChangedEvent(IEventPublisher eventPublisher, ILogger<PublishApprenticeshipChangedEvent> logger)
        {
            _eventPublisher = eventPublisher;
            _logger = logger;
        }

        public async Task Handle(ApprenticeshipChanged notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Publishing ApprenticeshipChangedEvent for {apprentice} -- {apprenticeship}",
                notification.Apprenticeship.ApprenticeId, notification.Apprenticeship.Id);

            await _eventPublisher.Publish(new ApprenticeshipChangedEvent
            {
                ApprenticeId = notification.Apprenticeship.ApprenticeId,
                ApprenticeshipId = notification.Apprenticeship.Id,
            });
        }
    }
}