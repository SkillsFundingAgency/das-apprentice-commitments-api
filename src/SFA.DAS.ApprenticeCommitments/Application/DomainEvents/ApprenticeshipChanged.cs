using MediatR;
using NServiceBus;
using SFA.DAS.ApprenticeCommitments.Data.Models;
using SFA.DAS.ApprenticeCommitments.Messages.Events;
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

        public ApprenticeshipChangedHandler(IMessageSession messageSession)
        {
            _messageSession = messageSession;
        }

        public async Task Handle(ApprenticeshipChanged notification, CancellationToken cancellationToken)
        {
            await _messageSession.Publish(new ApprenticeshipChangedEvent
            {
                ApprenticeId = notification.Apprenticeship.ApprenticeId,
                ApprenticeshipId = notification.Apprenticeship.Id,
            });
        }
    }
}