using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.ApprenticeCommitments.Data.Models;
using SFA.DAS.ApprenticeCommitments.Messages.Events;
using SFA.DAS.NServiceBus.Services;

namespace SFA.DAS.ApprenticeCommitments.Application.DomainEvents.Handlers
{
    internal class PublishApprenticeshipConfirmationCommencedEvent
        : INotificationHandler<RegistrationAdded>
        , INotificationHandler<RevisionAdded>
    {
        private readonly IEventPublisher _eventPublisher;
        private readonly ILogger<PublishApprenticeshipConfirmationCommencedEvent> _logger;

        public PublishApprenticeshipConfirmationCommencedEvent(
            IEventPublisher eventPublisher,
            ILogger<PublishApprenticeshipConfirmationCommencedEvent> logger)
        {
            _eventPublisher = eventPublisher;
            _logger = logger;
        }

        public async Task Handle(RegistrationAdded notification, CancellationToken cancellationToken)
        {
            // The revision isn't technically added until the apprentice
            // confirms their identity, however it's possible that the apprentice
            // never does.  Approvals can use this event to prompt the apprentice
            // when the confirmation is overdue.
            var pretend = new Revision(
                notification.Registration.CommitmentsApprenticeshipId,
                notification.Registration.CommitmentsApprovedOn,
                notification.Registration.Approval);

            _logger.LogInformation(
                "RegistrationAdded - Publishing ApprenticeshipConfirmationCommencedEvent for Apprentice {ApprenticeId}, Apprenticeship {ApprenticeshipId}",
                notification.Registration.RegistrationId,
                notification.Registration.CommitmentsApprenticeshipId);

            await _eventPublisher.Publish(new ApprenticeshipConfirmationCommencedEvent
            {
                ApprenticeId = notification.Registration.RegistrationId,
                ConfirmationOverdueOn = pretend.ConfirmBefore,
                CommitmentsApprenticeshipId = notification.Registration.CommitmentsApprenticeshipId,
                CommitmentsApprovedOn = notification.Registration.CommitmentsApprovedOn,
            });
        }

        public async Task Handle(RevisionAdded notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "RevisionAdded - Publishing ApprenticeshipConfirmationCommencedEvent for Apprentice {ApprenticeId}, Apprenticeship {ApprenticeshipId}",
                notification.Revision.Apprenticeship.ApprenticeId,
                notification.Revision.ApprenticeshipId);

            await _eventPublisher.Publish(new ApprenticeshipConfirmationCommencedEvent
            {
                ApprenticeId = notification.Revision.Apprenticeship.ApprenticeId,
                ApprenticeshipId = notification.Revision.ApprenticeshipId,
                ConfirmationId = notification.Revision.Id,
                ConfirmationOverdueOn = notification.Revision.ConfirmBefore,
                CommitmentsApprenticeshipId = notification.Revision.CommitmentsApprenticeshipId,
                CommitmentsApprovedOn = notification.Revision.CommitmentsApprovedOn,
            });
        }
    }
}