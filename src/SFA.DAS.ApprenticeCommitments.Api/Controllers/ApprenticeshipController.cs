using MediatR;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.ApprenticeCommitments.Application.Commands.ApprenticeshipShownCommand;
using SFA.DAS.ApprenticeCommitments.Application.Commands.ChangeApprenticeshipCommand;
using SFA.DAS.ApprenticeCommitments.Application.Commands.CreateRegistrationCommand;
using SFA.DAS.ApprenticeCommitments.Application.Queries.ApprenticeshipQuery;
using SFA.DAS.ApprenticeCommitments.Application.Queries.ApprenticeshipRevisionsQuery;
using SFA.DAS.ApprenticeCommitments.Data.Models;
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

        [HttpGet("apprentices/{apprenticeId}/apprenticeships/{apprenticeshipId}/revisions")]
        public async Task<IActionResult> GetApprenticeshipRevisions(Guid apprenticeId, long apprenticeshipId)
        {
            var result = await _mediator.Send(new ApprenticeshipRevisionsQuery(apprenticeId, apprenticeshipId));
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpPatch("apprentices/{apprenticeId}/apprenticeships/{apprenticeshipId}")]
        public async Task<IActionResult> PostApprenticeship(
            Guid apprenticeId,
            long apprenticeshipId,
            [FromBody] JsonPatchDocument<Apprenticeship> changes)
        {
            await _mediator.Send(new UpdateApprenticeshipCommand(apprenticeId, apprenticeshipId, changes));
            return Ok();
        }
    }
}