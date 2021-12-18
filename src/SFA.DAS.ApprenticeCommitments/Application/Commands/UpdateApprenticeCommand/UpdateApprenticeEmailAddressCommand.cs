using MediatR;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Logging;
using SFA.DAS.ApprenticeCommitments.Data;
using SFA.DAS.ApprenticeCommitments.DTOs;
using SFA.DAS.ApprenticeCommitments.Infrastructure.Mediator;
using SFA.DAS.ApprenticeCommitments.Messages.Events;
using SFA.DAS.NServiceBus.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Application.Commands.UpdateApprenticeCommand
{
    public class UpdateApprenticeEmailAddressCommand : IUnitOfWorkCommand
    {
        public UpdateApprenticeEmailAddressCommand(Guid apprenticeId, JsonPatchDocument<ApprenticePatchDto> updates)
        {
            ApprenticeId = apprenticeId;
            Updates = updates;
        }

        public Guid ApprenticeId { get; }
        public JsonPatchDocument<ApprenticePatchDto> Updates { get; }
    }

    public class UpdateApprenticeCommandHandler : IRequestHandler<UpdateApprenticeEmailAddressCommand>
    {
        private readonly IApprenticeshipContext _apprenticeships;
        private readonly IEventPublisher _eventPublisher;
        private readonly ILogger<UpdateApprenticeCommandHandler> _logger;

        public UpdateApprenticeCommandHandler(
            IApprenticeshipContext apprenticeships,
            IEventPublisher eventPublisher,
            ILogger<UpdateApprenticeCommandHandler> logger)
        {
            _apprenticeships = apprenticeships;
            _logger = logger;
            _eventPublisher = eventPublisher;
        }

        public async Task<Unit> Handle(UpdateApprenticeEmailAddressCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Processing ApprenticeEmailAddressChanged for {apprentice} ",
                request.ApprenticeId);

            var apprenticeships = await _apprenticeships.FindAllForApprentice(request.ApprenticeId);

            foreach (var apprenticeship in apprenticeships)
            {
                _logger.LogInformation(
                    "Processing ApprenticeshipEmailAddressChanged for {apprentice} - {apprenticeship} ",
                    request.ApprenticeId, apprenticeship.Id);

                await _eventPublisher.Publish(new ApprenticeshipEmailAddressChangedEvent
                {
                    ApprenticeId = apprenticeship.ApprenticeId,
                    CommitmentsApprenticeshipId = apprenticeship.LatestRevision.CommitmentsApprenticeshipId
                });
            }

            return Unit.Value;
        }
    }
}