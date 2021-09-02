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
    internal class PublishApprenticeshipRegisteredEvent
        : INotificationHandler<RegistrationAdded>
        , INotificationHandler<RegistrationUpdated>
    {
        private readonly IApprenticeContext apprentices;
        private readonly IMessageSession messageSession;
        private readonly ILogger<PublishApprenticeshipRegisteredEvent> logger;

        public PublishApprenticeshipRegisteredEvent(IApprenticeContext apprentices, IMessageSession messageSession, ILogger<PublishApprenticeshipRegisteredEvent> logger)
        {
            this.apprentices = apprentices;
            this.messageSession = messageSession;
            this.logger = logger;
        }

        public async Task Handle(RegistrationAdded notification, CancellationToken cancellationToken)
            => await Publish(notification.Registration, nameof(RegistrationAdded));

        public async Task Handle(RegistrationUpdated notification, CancellationToken cancellationToken)
        {
            var a = await apprentices.GetByEmail(notification.Registration.Email);
            if (a.Any()) return;

            await Publish(notification.Registration, nameof(RegistrationUpdated));
        }

        private async Task Publish(Registration registration, string eventName)
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
