using MediatR;
using NServiceBus;
using SFA.DAS.ApprenticeCommitments.Data.Models;
using SFA.DAS.ApprenticeCommitments.Messages.Events;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Application.DomainEvents
{
    internal class ApprenticeshipChanged : INotification
    {
        public Apprenticeship Apprenticeship { get; }

        public ApprenticeshipChanged(Apprenticeship apprenticeship)
            => Apprenticeship = apprenticeship;
    }

    internal class ApprenticeshipChangedHandler : INotificationHandler<ApprenticeshipChanged>
    {
        private readonly IMessageSession messageSession;

        public ApprenticeshipChangedHandler(IMessageSession messageSession)
        {
            this.messageSession = messageSession;
        }

        public async Task Handle(ApprenticeshipChanged notification, CancellationToken cancellationToken)
        {
            var ordered = notification.Apprenticeship.CommitmentStatements
                .OrderBy(x => x.CommitmentsApprovedOn).ToArray();

            var newest = ordered[^1];
            var previous = ordered[^2];

            var sinceLastApproval = newest.CommitmentsApprovedOn - previous.CommitmentsApprovedOn;
            var seenPreviousApproval = notification.Apprenticeship.LastViewed > previous.CommitmentsApprovedOn;

            if (sinceLastApproval > TimeSpan.FromHours(24) || seenPreviousApproval)
            {
                await messageSession.Publish(new ApprenticeshipChangedEvent
                {
                    ApprenticeId = notification.Apprenticeship.ApprenticeId,
                    ApprenticeshipId = notification.Apprenticeship.Id,
                });
            }
        }
    }
}