using MediatR;
using Microsoft.Extensions.Logging;
using NServiceBus;
using SFA.DAS.ApprenticeCommitments.Data;
using SFA.DAS.ApprenticeCommitments.Data.Models;
using SFA.DAS.ApprenticeCommitments.Messages.Events;
using System.Linq;
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
        private readonly IApprenticeContext apprentices;
        private readonly IMessageSession messageSession;
        private readonly ILogger<RegistrationUpdatedHandler> logger;

        public RegistrationUpdatedHandler(IApprenticeContext apprentices, IMessageSession messageSession, ILogger<RegistrationUpdatedHandler> logger)
        {
            this.apprentices = apprentices;
            this.messageSession = messageSession;
            this.logger = logger;
        }

        public async Task Handle(RegistrationAdded notification, CancellationToken cancellationToken)
            => await PublishApprenticeshipRegisteredEvent(notification.Registration, nameof(RegistrationAdded));

        public async Task Handle(RegistrationUpdated notification, CancellationToken cancellationToken)
        {
            var a = await apprentices.GetByEmail(notification.Registration.Email);
            if (a.Any()) return;

            await PublishApprenticeshipRegisteredEvent(notification.Registration, nameof(RegistrationUpdated));
        }

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
