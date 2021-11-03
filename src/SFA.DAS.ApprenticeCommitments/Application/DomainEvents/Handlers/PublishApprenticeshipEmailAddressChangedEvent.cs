using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using NServiceBus;
using SFA.DAS.ApprenticeCommitments.Messages.Events;

namespace SFA.DAS.ApprenticeCommitments.Application.DomainEvents.Handlers
{
    internal class PublishApprenticeshipEmailAddressChangedEvent : INotificationHandler<ApprenticeEmailAddressChanged>
    {
        private readonly IMessageSession _messageSession;
        private readonly ILogger<PublishApprenticeshipEmailAddressChangedEvent> _logger;

        public PublishApprenticeshipEmailAddressChangedEvent(IMessageSession messageSession, ILogger<PublishApprenticeshipEmailAddressChangedEvent> logger)
        {
            _messageSession = messageSession;
            _logger = logger;
        }

        public async Task Handle(ApprenticeEmailAddressChanged notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Processing ApprenticeEmailAddressChanged for {apprentice} ", notification.Apprentice.Id);

            foreach (var apprenticeship in notification.Apprentice.Apprenticeships)
            {
                _logger.LogInformation("Processing ApprenticeshipEmailAddressChanged for {apprentice} - {apprenticeship} ", notification.Apprentice.Id, apprenticeship.Id);
                await _messageSession.Publish(new ApprenticeshipEmailAddressChangedEvent
                {
                    ApprenticeId = apprenticeship.ApprenticeId,
                    CommitmentsApprenticeshipId = apprenticeship.LatestRevision.CommitmentsApprenticeshipId
                });
            }
        }
    }
}