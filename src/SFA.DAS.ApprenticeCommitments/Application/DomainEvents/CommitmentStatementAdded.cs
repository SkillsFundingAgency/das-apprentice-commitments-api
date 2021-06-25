using MediatR;
using NServiceBus;
using SFA.DAS.ApprenticeCommitments.Data.Models;
using SFA.DAS.ApprenticeCommitments.Messages.Events;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Application.DomainEvents
{
    internal class CommitmentStatementAdded : INotification
    {
        public CommitmentStatement CommitmentStatement { get; }

        public CommitmentStatementAdded(CommitmentStatement commitmentStatement)
            => CommitmentStatement = commitmentStatement;
    }

    internal class ApprenticeshipAddedHandler : INotificationHandler<CommitmentStatementAdded>
    {
        private readonly IMessageSession messageSession;

        public ApprenticeshipAddedHandler(IMessageSession messageSession)
            => this.messageSession = messageSession;

        public async Task Handle(CommitmentStatementAdded notification, CancellationToken cancellationToken)
        {
            await messageSession.Publish(new ApprenticeshipConfirmationCommencedEvent
            {
                ApprenticeId = notification.CommitmentStatement.Apprenticeship.ApprenticeId,
                ApprenticeshipId = notification.CommitmentStatement.ApprenticeshipId,
                ConfirmationId = notification.CommitmentStatement.Id,
                ConfirmationOverdueOn = notification.CommitmentStatement.ConfirmBefore,
                CommitmentsApprenticeshipId = notification.CommitmentStatement.CommitmentsApprenticeshipId,
                CommitmentsApprovedOn = notification.CommitmentStatement.CommitmentsApprovedOn,
            });
        }
    }
}