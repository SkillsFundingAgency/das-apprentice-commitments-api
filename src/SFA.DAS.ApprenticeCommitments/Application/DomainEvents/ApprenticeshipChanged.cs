using MediatR;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<ApprenticeshipChangedHandler> _logger;

        public ApprenticeshipChangedHandler(IMessageSession messageSession, ILogger<ApprenticeshipChangedHandler> logger)
        {
            _messageSession = messageSession;
            _logger = logger;
        }

        public async Task Handle(ApprenticeshipChanged notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Publishing ApprenticeshipChangedEvent for {apprentice} -- {apprenticeship}",
                notification.Apprenticeship.ApprenticeId, notification.Apprenticeship.Id);

            await _messageSession.Publish(new ApprenticeshipChangedEvent
            {
                ApprenticeId = notification.Apprenticeship.ApprenticeId,
                ApprenticeshipId = notification.Apprenticeship.Id,
            });
        }
    }
}