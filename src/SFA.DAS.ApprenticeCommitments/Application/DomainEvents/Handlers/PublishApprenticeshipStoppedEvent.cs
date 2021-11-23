using MediatR;
using Microsoft.Extensions.Logging;
using NServiceBus;
using SFA.DAS.ApprenticeCommitments.Messages.Events;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Application.DomainEvents
{
    internal class PublishApprenticeshipStoppedEvent : INotificationHandler<ApprenticeshipStopped>
    {
        private readonly IMessageSession messageSession;
        private readonly ILogger<PublishApprenticeshipStoppedEvent> logger;

        public PublishApprenticeshipStoppedEvent(IMessageSession messageSession, ILogger<PublishApprenticeshipStoppedEvent> logger)
        {
            this.messageSession = messageSession;
            this.logger = logger;
        }

        public async Task Handle(ApprenticeshipStopped notification, CancellationToken cancellationToken)
        {
            logger.LogInformation(
                "ApprenticeshipStopped - Publishing ApprenticeshipStopped for Apprenticeship {ApprenticeshipId}, CommitmentsApprenticeshipId {CommitmentsApprenticeshipId}",
                notification.Revision.ApprenticeshipId,
                notification.Revision.CommitmentsApprenticeshipId
                );

            await messageSession.Publish(new ApprenticeshipStoppedEvent
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