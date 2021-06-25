using MediatR;
using NServiceBus;
using SFA.DAS.ApprenticeCommitments.Data.Models;
using SFA.DAS.ApprenticeCommitments.Messages.Events;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Application.DomainEvents
{
    internal class RegistrationAdded : INotification
    {
        public Registration Registration { get; }

        public RegistrationAdded(Registration registration)
            => Registration = registration;
    }

    internal class RegistrationAddedHandler : INotificationHandler<RegistrationAdded>
    {
        private readonly IMessageSession messageSession;

        public RegistrationAddedHandler(IMessageSession messageSession)
            => this.messageSession = messageSession;

        public async Task Handle(RegistrationAdded notification, CancellationToken cancellationToken)
        {
            // The commitment statement isn't technically added until the apprentice
            // confirms their identity, however it's possible that the apprentice
            // never does.  Approvals can use this event to prompt the apprentice
            // when the confirmation is overdue.
            var pretend = new CommitmentStatement(
                notification.Registration.CommitmentsApprenticeshipId,
                notification.Registration.CommitmentsApprovedOn,
                notification.Registration.Apprenticeship);

            await messageSession.Publish(new ApprenticeshipConfirmationCommencedEvent
            {
                ApprenticeId = notification.Registration.ApprenticeId,
                ConfirmationOverdueOn = pretend.ConfirmBefore,
                CommitmentsApprenticeshipId = notification.Registration.CommitmentsApprenticeshipId,
                CommitmentsApprovedOn = notification.Registration.CommitmentsApprovedOn,
            });
        }
    }
}