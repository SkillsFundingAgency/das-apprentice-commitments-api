using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.ApprenticeCommitments.Messages.Events;
using SFA.DAS.NServiceBus.Services;

namespace SFA.DAS.ApprenticeCommitments.Application.DomainEvents.Handlers
{
    internal class PublishApprenticeshipStoppedEvent : INotificationHandler<ApprenticeshipStopped>
    {
        private readonly IEventPublisher _eventPublisher;
        private readonly ILogger<PublishApprenticeshipStoppedEvent> _logger;

        public PublishApprenticeshipStoppedEvent(IEventPublisher eventPublisher, ILogger<PublishApprenticeshipStoppedEvent> logger)
        {
            _eventPublisher = eventPublisher;
            _logger = logger;
        }

        public async Task Handle(ApprenticeshipStopped notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "ApprenticeshipStopped - Publishing ApprenticeshipStopped for Apprenticeship {ApprenticeshipId}, CommitmentsApprenticeshipId {CommitmentsApprenticeshipId}",
                notification.Revision.ApprenticeshipId,
                notification.Revision.CommitmentsApprenticeshipId
                );

            await _eventPublisher.Publish(new ApprenticeshipStoppedEvent
            {
                ApprenticeshipId = notification.Revision.ApprenticeshipId,
                CommitmentsApprenticeshipId = notification.Revision.CommitmentsApprenticeshipId,
                ApprenticeId = notification.Revision.Apprenticeship.ApprenticeId,
                CourseName = notification.Revision.Details.Course.Name,
                EmployerName = notification.Revision.Details.EmployerName,
            });
        }
    }
}