using MediatR;
using Microsoft.Extensions.Logging;
using NServiceBus;
using SFA.DAS.ApprenticeCommitments.Data.Models;
using SFA.DAS.ApprenticeCommitments.Messages.Events;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Application.DomainEvents
{
    public class RegistrationUpdated : INotification
    {
        public Registration Registration { get; }

        public RegistrationUpdated(Registration registration)
            => Registration = registration;
    }

    internal class RegistrationUpdatedHandler
        : INotificationHandler<RegistrationAdded>
        , INotificationHandler<RegistrationUpdated>
    {
        private readonly IMessageSession messageSession;
        private readonly ILogger<RegistrationAddedHandler> logger;

        public RegistrationUpdatedHandler(IMessageSession messageSession, ILogger<RegistrationAddedHandler> logger)
        {
            this.messageSession = messageSession;
            this.logger = logger;
        }

        public async Task Handle(RegistrationAdded notification, CancellationToken cancellationToken)
            => await PublishApprenticeshipRegisteredEvent(notification.Registration, nameof(RegistrationAdded));

        public async Task Handle(RegistrationUpdated notification, CancellationToken cancellationToken)
            => await PublishApprenticeshipRegisteredEvent(notification.Registration, nameof(RegistrationUpdated));

        private async Task PublishApprenticeshipRegisteredEvent(Registration registration, string eventName)
        {
            logger.LogInformation(
                            "{DomainEvent} - Publishing ApprenticeshipRegisteredEvent for Registration {RegistrationId}, CommitmentsApprenticeshipId {CommitmentsApprenticeshipId}",
                            eventName,
                            registration.RegistrationId,
                            registration.CommitmentsApprenticeshipId);

            await messageSession.Publish(new ApprenticeshipRegisteredEvent
            {
                RegistrationId = registration.RegistrationId,
            });
        }
    }
}
