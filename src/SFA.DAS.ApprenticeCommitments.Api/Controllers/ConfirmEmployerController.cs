using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.ApprenticeCommitments.Application.Commands.ConfirmTrainingProviderCommand;
using System;
using System.Threading.Tasks;
using SFA.DAS.ApprenticeCommitments.Application.Commands.ConfirmEmployerCommand;

namespace SFA.DAS.ApprenticeCommitments.Api.Controllers
{

    [ApiController]
    public class ConfirmEmployerController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ConfirmEmployerController(IMediator mediator) => _mediator = mediator;

        [HttpPost("apprentices/{apprenticeId}/apprenticeships/{apprenticeshipId}/EmployerConfirmation")]
        public async Task<IActionResult> ConfirmTrainingProvider(
            Guid apprenticeId, long apprenticeshipId,
            [FromBody] ConfirmEmployerData data)
        {
            var command = new ConfirmEmployerCommand(apprenticeId, apprenticeshipId, data);
            await _mediator.Send(command);
            return Ok();
        }
    }
}