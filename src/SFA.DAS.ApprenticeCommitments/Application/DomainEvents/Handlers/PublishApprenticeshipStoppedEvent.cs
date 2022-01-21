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
                "ApprenticeshipStopped - Publishing ApprenticeshipStopped for Apprenticeship {ApprenticeshipId}, CommitmentsApprenticeshipId {CommitmentsApprenticeshipId}",
                notification.Registration.Apprenticeship?.Id,
                notification.Registration.CommitmentsApprenticeshipId
                );

            await _eventPublisher.Publish(new ApprenticeshipStoppedEvent
            {
                ApprenticeshipId = notification.Registration.Apprenticeship?.Id,
                CommitmentsApprenticeshipId = notification.Registration.CommitmentsApprenticeshipId,
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
                ApprenticeshipId = notification.Revision.Apprenticeship?.Id,
                CommitmentsApprenticeshipId = notification.Revision.CommitmentsApprenticeshipId,
                ApprenticeId = notification.Revision.Apprenticeship.ApprenticeId,
                CourseName = notification.Revision.Details.Course.Name,
                EmployerName = notification.Revision.Details.EmployerName,
            });
        }
    }
}