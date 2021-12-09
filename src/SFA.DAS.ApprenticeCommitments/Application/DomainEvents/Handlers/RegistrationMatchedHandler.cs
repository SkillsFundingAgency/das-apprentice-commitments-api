﻿using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.ApprenticeCommitments.Messages.Events;
using SFA.DAS.NServiceBus.Services;

namespace SFA.DAS.ApprenticeCommitments.Application.DomainEvents.Handlers
{
    internal class RegistrationMatchedHandler : INotificationHandler<RegistrationMatched>
    {
        private readonly IEventPublisher _eventPublisher;
        private readonly ILogger<RegistrationMatchedHandler> _logger;

        public RegistrationMatchedHandler(IEventPublisher eventPublisher, ILogger<RegistrationMatchedHandler> logger)
        {
            _eventPublisher = eventPublisher;
            _logger = logger;
        }

        public async Task Handle(RegistrationMatched notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "RegistrationMatched - Publishing ApprenticeshipEmailAddressConfirmedEvent for Apprentice {ApprenticeId}, CommitmentApprenticeship {CommitmentsApprenticeshipId}",
                notification.Apprentice.Id,
                notification.Registration.CommitmentsApprenticeshipId);

            await _eventPublisher.Publish(new ApprenticeshipEmailAddressConfirmedEvent
            {
                ApprenticeId = notification.Apprentice.Id,
                CommitmentsApprenticeshipId = notification.Registration.CommitmentsApprenticeshipId,
            });
        }
    }
}