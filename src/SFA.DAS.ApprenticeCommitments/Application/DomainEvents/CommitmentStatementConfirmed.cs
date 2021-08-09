using MediatR;
using Microsoft.Extensions.Logging;
using NServiceBus;
using SFA.DAS.ApprenticeCommitments.Data.Models;
using SFA.DAS.ApprenticeCommitments.Exceptions;
using SFA.DAS.ApprenticeCommitments.Messages.Events;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Application.DomainEvents
{
    internal class CommitmentStatementConfirmed : INotification
    {
        public Revision CommitmentStatement { get; }

        public CommitmentStatementConfirmed(Revision commitmentStatement)
            => CommitmentStatement = commitmentStatement;
    }

    internal class CommitmentStatementConfirmedHandler : INotificationHandler<CommitmentStatementConfirmed>
    {
        private readonly IMessageSession messageSession;
        private readonly ILogger<CommitmentStatementConfirmedHandler> logger;

        public CommitmentStatementConfirmedHandler(IMessageSession messageSession, ILogger<CommitmentStatementConfirmedHandler> logger)
        {
            this.messageSession = messageSession;
            this.logger = logger;
        }

        public async Task Handle(CommitmentStatementConfirmed notification, CancellationToken cancellationToken)
        {
            if (notification.CommitmentStatement.ConfirmedOn == null)
                throw new DomainException($"Commitment statement {notification.CommitmentStatement.Id} for apprenticeship {notification.CommitmentStatement.ApprenticeshipId} has not been confirmed");

            logger.LogInformation(
                "Publishing ApprenticeshipConfirmationConfirmedEvent for Apprentice {ApprenticeId}, Apprenticeship {ApprenticeshipId}, confirmed on {ConfirmedOn}",
                notification.CommitmentStatement.Apprenticeship.ApprenticeId,
                notification.CommitmentStatement.ApprenticeshipId,
                notification.CommitmentStatement.ConfirmedOn.Value);

            await messageSession.Publish(new ApprenticeshipConfirmationConfirmedEvent
            {
                ApprenticeId = notification.CommitmentStatement.Apprenticeship.ApprenticeId,
                ApprenticeshipId = notification.CommitmentStatement.ApprenticeshipId,
                ConfirmationId = notification.CommitmentStatement.Id,
                ConfirmedOn = notification.CommitmentStatement.ConfirmedOn.Value,
                CommitmentsApprenticeshipId = notification.CommitmentStatement.CommitmentsApprenticeshipId,
                CommitmentsApprovedOn = notification.CommitmentStatement.CommitmentsApprovedOn,
            });
        }
    }
}