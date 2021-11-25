using MediatR;
using Microsoft.Extensions.Logging;
using NServiceBus;
using SFA.DAS.ApprenticeCommitments.Data.Models;
using SFA.DAS.ApprenticeCommitments.Messages.Events;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Application.DomainEvents
{
    internal class RegistrationMatched : INotification
    {
        public Apprenticeship Apprenticeship { get; }

        public RegistrationMatched(Apprenticeship apprenticeship)
        {
            Apprenticeship = apprenticeship;
        }
    }

    internal class RegistrationMatchedHandler : INotificationHandler<RegistrationMatched>
    {
        private readonly IMessageSession messageSession;
        private readonly ILogger<RegistrationMatchedHandler> logger;

        public RegistrationMatchedHandler(IMessageSession messageSession, ILogger<RegistrationMatchedHandler> logger)
        {
            this.messageSession = messageSession;
            this.logger = logger;
        }

        public async Task Handle(RegistrationMatched notification, CancellationToken cancellationToken)
        {
            logger.LogInformation(
                "RegistrationMatched - Publishing ApprenticeshipEmailAddressConfirmedEvent for Apprentice {ApprenticeId}, CommitmentApprenticeship {CommitmentsApprenticeshipId}",
                notification.Apprenticeship.ApprenticeId,
                notification.Apprenticeship.LatestRevision.CommitmentsApprenticeshipId);

            await messageSession.Publish(new ApprenticeshipEmailAddressConfirmedEvent
            {
                ApprenticeId = notification.Apprenticeship.ApprenticeId,
                CommitmentsApprenticeshipId = notification.Apprenticeship.LatestRevision.CommitmentsApprenticeshipId,
            });
        }
    }
}