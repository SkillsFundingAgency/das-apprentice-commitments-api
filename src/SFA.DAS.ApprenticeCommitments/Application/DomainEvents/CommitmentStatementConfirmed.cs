using MediatR;
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
        public CommitmentStatement CommitmentStatement { get; }

        public CommitmentStatementConfirmed(CommitmentStatement commitmentStatement)
            => CommitmentStatement = commitmentStatement;
    }

    internal class CommitmentStatementConfirmedHandler : INotificationHandler<CommitmentStatementConfirmed>
    {
        private readonly IMessageSession messageSession;

        public CommitmentStatementConfirmedHandler(IMessageSession messageSession)
            => this.messageSession = messageSession;

        public async Task Handle(CommitmentStatementConfirmed notification, CancellationToken cancellationToken)
        {
            if (notification.CommitmentStatement.ConfirmedOn == null)
                throw new DomainException($"Commitment statement {notification.CommitmentStatement.Id} for apprenticeship {notification.CommitmentStatement.ApprenticeshipId} has not been confirmed");

            await messageSession.Publish(new ApprenticeshipConfirmationConfirmedEvent
            {
                ApprenticeshipId = notification.CommitmentStatement.ApprenticeshipId,
                ConfirmationId = notification.CommitmentStatement.Id,
                ConfirmedOn = notification.CommitmentStatement.ConfirmedOn.Value,
                CommitmentsApprenticeshipId = notification.CommitmentStatement.CommitmentsApprenticeshipId,
                CommitmentsApprovedOn = notification.CommitmentStatement.CommitmentsApprovedOn,
            });
        }
    }
}