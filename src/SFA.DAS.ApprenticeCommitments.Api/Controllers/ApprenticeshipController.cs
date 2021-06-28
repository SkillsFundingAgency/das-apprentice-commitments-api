using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.ApprenticeCommitments.Application.Commands.ApprenticeshipShownCommand;
using SFA.DAS.ApprenticeCommitments.Application.Commands.ChangeApprenticeshipCommand;
using SFA.DAS.ApprenticeCommitments.Application.Commands.CreateRegistrationCommand;
using SFA.DAS.ApprenticeCommitments.Application.Queries.ApprenticeshipQuery;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Api.Controllers
{
    [ApiController]
    public class ApprenticeshipController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ApprenticeshipController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("apprenticeships")]
        public async Task<IActionResult> CreateRegistration([FromBody] CreateRegistrationCommand request)
        {
            await _mediator.Send(request);
            return Accepted();
        }

        [HttpPost("apprenticeships/change")]
        public async Task<IActionResult> ChangeApprenticeship([FromBody] ChangeApprenticeshipCommand request)
        {
            await _mediator.Send(request);
            return Accepted();
        }

        [HttpGet("apprentices/{apprenticeId}/apprenticeships/{apprenticeshipId}")]
        public async Task<IActionResult> GetApprenticeship(Guid apprenticeId, long apprenticeshipId)
        {
            var result = await _mediator.Send(new ApprenticeshipQuery(apprenticeId, apprenticeshipId));
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpPost("apprentices/{apprenticeId}/apprenticeships/{apprenticeshipId}/visits")]
        public async Task<IActionResult> PostApprenticeship(Guid apprenticeId, long apprenticeshipId)
        {
            await _mediator.Send(new ApprenticeshipShownCommand(apprenticeId, apprenticeshipId));
            return Ok();
        }
    }
}