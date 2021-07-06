using MediatR;
using NServiceBus;
using SFA.DAS.ApprenticeCommitments.Configuration;
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
        private readonly IMessageSession _messageSession;
        private readonly TimeSpan _timeToWaitBeforeEmail;

        public ApprenticeshipChangedHandler(IMessageSession messageSession, ApplicationSettings settings)
        {
            _messageSession = messageSession;
            _timeToWaitBeforeEmail = settings.TimeToWaitBeforeChangeOfApprenticeshipEmail;
        }

        public async Task Handle(ApprenticeshipChanged notification, CancellationToken cancellationToken)
        {
            var ordered = notification.Apprenticeship.CommitmentStatements
                .OrderBy(x => x.CommitmentsApprovedOn).ToArray();

            var newest = ordered[^1];
            var previous = ordered[^2];

            var sinceLastApproval = newest.CommitmentsApprovedOn - previous.CommitmentsApprovedOn;
            var seenPreviousApproval = notification.Apprenticeship.LastViewed > previous.CommitmentsApprovedOn;

            if (sinceLastApproval > _timeToWaitBeforeEmail || seenPreviousApproval)
            {
                await _messageSession.Publish(new ApprenticeshipChangedEvent
                {
                    ApprenticeId = notification.Apprenticeship.ApprenticeId,
                    ApprenticeshipId = notification.Apprenticeship.Id,
                });
            }
        }
    }
}