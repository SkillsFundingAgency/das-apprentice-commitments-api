using MediatR;
using NServiceBus;
using SFA.DAS.ApprenticeCommitments.Messages.Events;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Application.DomainEvents
{
    internal class ApprenticeshipConfirmationCommenced : ApprenticeshipConfirmationCommencedEvent, INotification
    {
    }

    internal class ApprenticeshipConfirmationCommencedHandler : INotificationHandler<ApprenticeshipConfirmationCommenced>
    {
        private readonly IMessageSession messageSession;

        public ApprenticeshipConfirmationCommencedHandler(IMessageSession messageSession)
        {
            this.messageSession = messageSession;
        }

        public async Task Handle(ApprenticeshipConfirmationCommenced notification, CancellationToken cancellationToken)
        {
            await messageSession.Publish(notification as ApprenticeshipConfirmationCommencedEvent);
        }
    }
}