using MediatR;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.ApprenticeCommitments.Application.Commands.CreateApprenticeAccountCommand;
using SFA.DAS.ApprenticeCommitments.Data;
using SFA.DAS.ApprenticeCommitments.DTOs;
using SFA.DAS.ApprenticeCommitments.Infrastructure.Mediator;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Application.Commands.UpdateApprenticeCommand
{
    public class UpdateApprenticeCommand : IUnitOfWorkCommand
    {
        public UpdateApprenticeCommand(Guid apprenticeId, JsonPatchDocument<ApprenticePatchDto> updates)
        {
            ApprenticeId = apprenticeId;
            Updates = updates;
        }

        public Guid ApprenticeId { get; }
        public JsonPatchDocument<ApprenticePatchDto> Updates { get; }
    }

    public class UpdateApprenticeCommandHandler : IRequestHandler<UpdateApprenticeCommand>
    {
        private readonly IApprenticeContext _apprentices;
        private readonly ILogger<UpdateApprenticeCommandHandler> _logger;

        public UpdateApprenticeCommandHandler(IApprenticeContext apprenticeships, ILogger<UpdateApprenticeCommandHandler> logger)
        {
            _apprentices = apprenticeships;
            _logger = logger;
        }

        public async Task<Unit> Handle(UpdateApprenticeCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Updating {request.ApprenticeId} - {JsonConvert.SerializeObject(request.Updates)}");
            var app = await _apprentices.GetByIdAndIncludeApprenticeships(request.ApprenticeId);
            request.Updates.ApplyTo(new ApprenticePatchDto(app, _logger));
            var validation = new ApprenticeValidator().Validate(app);
            if (!validation.IsValid) throw new FluentValidation.ValidationException(validation.Errors);
            return Unit.Value;
        }
    }
}