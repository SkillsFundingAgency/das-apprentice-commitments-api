using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using NServiceBus;
using SFA.DAS.ApprenticeCommitments.Messages.Events;
using SFA.DAS.NServiceBus.Services;

namespace SFA.DAS.ApprenticeCommitments.Application.DomainEvents.Handlers
{
    internal class RegistrationMatchedHandler : INotificationHandler<RegistrationMatched>
    {
        private readonly IMessageSession _eventPublisher;
        private readonly ILogger<RegistrationMatchedHandler> _logger;

        public RegistrationMatchedHandler(IMessageSession eventPublisher, ILogger<RegistrationMatchedHandler> logger)
        {
            _eventPublisher = eventPublisher;
            _logger = logger;
        }

        public async Task Handle(RegistrationMatched notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "RegistrationMatched - Publishing ApprenticeshipEmailAddressConfirmedEvent for Apprentice {ApprenticeId}, CommitmentApprenticeship {CommitmentsApprenticeshipId}",
                notification.Apprenticeship.ApprenticeId,
                notification.Apprenticeship.LatestRevision.CommitmentsApprenticeshipId);

            await _eventPublisher.Publish(new ApprenticeshipEmailAddressConfirmedEvent
            {
                ApprenticeId = notification.Apprenticeship.ApprenticeId,
                CommitmentsApprenticeshipId = notification.Apprenticeship.LatestRevision.CommitmentsApprenticeshipId,
            });
        }
    }
}