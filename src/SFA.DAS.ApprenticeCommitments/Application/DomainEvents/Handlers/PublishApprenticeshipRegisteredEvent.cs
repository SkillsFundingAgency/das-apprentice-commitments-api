using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.ApprenticeCommitments.Data.Models;
using SFA.DAS.ApprenticeCommitments.Messages.Events;
using SFA.DAS.NServiceBus.Services;

namespace SFA.DAS.ApprenticeCommitments.Application.DomainEvents.Handlers
{
    internal class PublishApprenticeshipRegisteredEvent
        : INotificationHandler<RegistrationAdded>
        , INotificationHandler<RegistrationUpdated>
    {
        private readonly IEventPublisher _eventPublisher;
        private readonly ILogger<PublishApprenticeshipRegisteredEvent> _logger;

        public PublishApprenticeshipRegisteredEvent(IEventPublisher eventPublisher, ILogger<PublishApprenticeshipRegisteredEvent> logger)
        {
            _eventPublisher = eventPublisher;
            _logger = logger;
        }

        public async Task Handle(RegistrationAdded notification, CancellationToken cancellationToken)
            => await Publish(notification.Registration, nameof(RegistrationAdded));

        public async Task Handle(RegistrationUpdated notification, CancellationToken cancellationToken)
        {
            await Publish(notification.Registration, nameof(RegistrationUpdated));
        }

        private async Task Publish(Registration registration, string eventName)
        {
            _logger.LogInformation(
                            "{DomainEvent} - Publishing ApprenticeshipRegisteredEvent for Registration {RegistrationId}, CommitmentsApprenticeshipId {CommitmentsApprenticeshipId}",
                            eventName,
                            registration.RegistrationId,
                            registration.CommitmentsApprenticeshipId);

            await _eventPublisher.Publish(new ApprenticeshipRegisteredEvent
            {
                RegistrationId = registration.RegistrationId,
            });
        }
    }
}
