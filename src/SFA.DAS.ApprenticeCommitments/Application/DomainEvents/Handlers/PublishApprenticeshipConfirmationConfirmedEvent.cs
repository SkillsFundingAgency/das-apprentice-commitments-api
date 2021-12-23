using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.ApprenticeCommitments.Exceptions;
using SFA.DAS.ApprenticeCommitments.Messages.Events;
using SFA.DAS.NServiceBus.Services;

namespace SFA.DAS.ApprenticeCommitments.Application.DomainEvents.Handlers
{
    internal class PublishApprenticeshipConfirmationConfirmedEvent : INotificationHandler<RevisionConfirmed>
    {
        private readonly IEventPublisher _eventPublisher;
        private readonly ILogger<PublishApprenticeshipConfirmationConfirmedEvent> _logger;

        public PublishApprenticeshipConfirmationConfirmedEvent(
            IEventPublisher eventPublisher,
            ILogger<PublishApprenticeshipConfirmationConfirmedEvent> logger)
        {
            _eventPublisher = eventPublisher;
            _logger = logger;
        }

        public async Task Handle(RevisionConfirmed notification, CancellationToken cancellationToken)
        {
            if (notification.Revision.ConfirmedOn == null)
                throw new DomainException($"Apprenticeship {notification.Revision.ApprenticeshipId} revision {notification.Revision.Id} has not been confirmed");

            _logger.LogInformation(
                "Publishing ApprenticeshipConfirmationConfirmedEvent for Apprentice {ApprenticeId}, Apprenticeship {ApprenticeshipId}, confirmed on {ConfirmedOn}",
                notification.Revision.Apprenticeship.ApprenticeId,
                notification.Revision.ApprenticeshipId,
                notification.Revision.ConfirmedOn.Value);

            await _eventPublisher.Publish(new ApprenticeshipConfirmationConfirmedEvent
            {
                ApprenticeId = notification.Revision.Apprenticeship.ApprenticeId,
                ApprenticeshipId = notification.Revision.ApprenticeshipId,
                ConfirmationId = notification.Revision.Id,
                ConfirmedOn = notification.Revision.ConfirmedOn.Value,
                CommitmentsApprenticeshipId = notification.Revision.CommitmentsApprenticeshipId,
                CommitmentsApprovedOn = notification.Revision.CommitmentsApprovedOn,
            });
        }
    }
}