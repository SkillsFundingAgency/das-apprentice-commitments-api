using MediatR;
using Microsoft.Extensions.Logging;
using NServiceBus;
using SFA.DAS.ApprenticeCommitments.Data.Models;
using SFA.DAS.ApprenticeCommitments.Messages.Events;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Application.DomainEvents
{
    internal class RevisionAdded : INotification
    {
        public Revision Revision { get; }

        public RevisionAdded(Revision revision)
            => Revision = revision;
    }

    internal class RevisionAddedHandler : INotificationHandler<RevisionAdded>
    {
        private readonly IMessageSession messageSession;
        private readonly ILogger<RevisionAddedHandler> logger;

        public RevisionAddedHandler(IMessageSession messageSession, ILogger<RevisionAddedHandler> logger)
        {
            this.messageSession = messageSession;
            this.logger = logger;
        }

        public async Task Handle(RevisionAdded notification, CancellationToken cancellationToken)
        {
            logger.LogInformation(
                "RevisionAdded - Publishing ApprenticeshipConfirmationCommencedEvent for Apprentice {ApprenticeId}, Apprenticeship {ApprenticeshipId}",
                notification.Revision.Apprenticeship.ApprenticeId,
                notification.Revision.ApprenticeshipId);

            await messageSession.Publish(new ApprenticeshipConfirmationCommencedEvent
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