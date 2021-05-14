using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.ApprenticeCommitments.Application.Commands.ConfirmEmployerCommand;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Api.Controllers
{
    public class ConfirmEmployerRequest
    {
        public bool EmployerCorrect { get; set; }
    }

    [ApiController]
    public class ConfirmEmployerController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ConfirmEmployerController(IMediator mediator) => _mediator = mediator;

        [HttpPost("apprentices/{apprenticeId}/apprenticeships/{apprenticeshipId}/EmployerConfirmation")]
        public async Task<IActionResult> ConfirmTrainingProvider(
            Guid apprenticeId, long apprenticeshipId,
            [FromBody] ConfirmEmployerRequest request)
        {
            var command = new ConfirmEmployerCommand(apprenticeId, apprenticeshipId, request.EmployerCorrect);
            await _mediator.Send(command);
            return Ok();
        }
    }
}