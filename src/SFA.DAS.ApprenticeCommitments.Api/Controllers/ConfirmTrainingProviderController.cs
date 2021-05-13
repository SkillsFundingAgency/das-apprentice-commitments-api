using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.ApprenticeCommitments.Application.Commands.ConfirmTrainingProviderCommand;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Api.Controllers
{
    public class ConfirmTrainingProviderRequest
    {
        public bool TrainingProviderCorrect { get; set; }
    }

    [ApiController]
    public class ConfirmTrainingProviderController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ConfirmTrainingProviderController(IMediator mediator) => _mediator = mediator;

        [HttpPost("apprentices/{apprenticeId}/apprenticeships/{apprenticeshipId}/TrainingProviderConfirmation")]
        [HttpPost("apprentices/{apprenticeId}/apprenticeships/{apprenticeshipId}/{commitmentStatementId}/TrainingProviderConfirmation")]
        public async Task ConfirmTrainingProvider(
            Guid apprenticeId, long apprenticeshipId, long commitmentStatementId,
            [FromBody] ConfirmTrainingProviderRequest request)
        {
            await _mediator.Send(new ConfirmTrainingProviderCommand(
                (apprenticeId, apprenticeshipId, commitmentStatementId),
                request.TrainingProviderCorrect));
        }
    }
}