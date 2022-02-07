using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.ApprenticeCommitments.Messages.Events;
using SFA.DAS.NServiceBus.Services;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Application.DomainEvents.Handlers
{
    internal class PublishApprenticeshipStoppedEvent : INotificationHandler<RegistrationStopped>, INotificationHandler<RevisionStopped>
    {
        private readonly IEventPublisher _eventPublisher;
        private readonly ILogger<PublishApprenticeshipStoppedEvent> _logger;

        public PublishApprenticeshipStoppedEvent(IEventPublisher eventPublisher, ILogger<PublishApprenticeshipStoppedEvent> logger)
        {
            _eventPublisher = eventPublisher;
            _logger = logger;
        }

        public async Task Handle(RegistrationStopped notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "ApprenticeshipStopped - Publishing ApprenticeshipStopped for Registration {RegistrationId}, CommitmentsApprenticeshipId {CommitmentsApprenticeshipId}",
                notification.Registration.RegistrationId,
                notification.Registration.CommitmentsApprenticeshipId
                );

            await _eventPublisher.Publish(new ApprenticeshipStoppedEvent
            {
                CommitmentsApprenticeshipId = notification.Registration.CommitmentsApprenticeshipId,
                RegistrationId = notification.Registration.RegistrationId,
                ApprenticeId = notification.Registration.ApprenticeId,
                CourseName = notification.Registration.Approval.Course.Name,
                EmployerName = notification.Registration.Approval.EmployerName,
            });
        }

        public async Task Handle(RevisionStopped notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "RevisionStopped - Publishing ApprenticeshipStopped for Apprenticeship {ApprenticeshipId}, CommitmentsApprenticeshipId {CommitmentsApprenticeshipId}",
                notification.Revision.Apprenticeship.Id,
                notification.Revision.CommitmentsApprenticeshipId
                );

            await _eventPublisher.Publish(new ApprenticeshipStoppedEvent
            {
                CommitmentsApprenticeshipId = notification.Revision.CommitmentsApprenticeshipId,
                ApprenticeId = notification.Revision.Apprenticeship.ApprenticeId,
                ApprenticeshipId = notification.Revision.Apprenticeship.Id,
                CourseName = notification.Revision.Details.Course.Name,
                EmployerName = notification.Revision.Details.EmployerName,
            });
        }
    }
}