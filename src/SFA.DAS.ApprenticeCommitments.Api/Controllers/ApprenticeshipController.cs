using MediatR;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.ApprenticeCommitments.Application.Commands.UpdateApprenticeshipCommand;
using SFA.DAS.ApprenticeCommitments.Application.Commands.ChangeApprenticeshipCommand;
using SFA.DAS.ApprenticeCommitments.Application.Commands.ConfirmCommand;
using SFA.DAS.ApprenticeCommitments.Application.Commands.CreateApprenticeshipFromRegistrationCommand;
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
        public async Task CreateApprenticeship([FromBody] CreateApprenticeshipFromRegistrationCommand request)
            => await _mediator.Send(request);

        [HttpPost("apprenticeships/change")]
        public async Task ChangeApprenticeship([FromBody] ChangeApprenticeshipCommand request)
            => await _mediator.Send(request);

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
        public async Task PatchApprenticeship(
            Guid apprenticeId,
            long apprenticeshipId,
            [FromBody] JsonPatchDocument<Apprenticeship> changes)
        {
            await _mediator.Send(new UpdateApprenticeshipCommand(apprenticeId, apprenticeshipId, changes));
        }

        [HttpPatch("apprentices/{apprenticeId}/apprenticeships/{apprenticeshipId}/revisions/{revisionId}/confirmations")]
        public async Task ConfirmApprenticeship(
            Guid apprenticeId,
            long apprenticeshipId,
            long revisionId,
            [FromBody] Confirmations confirmations)
        {
            await _mediator.Send(new ConfirmCommand(apprenticeId, apprenticeshipId, revisionId, confirmations));
        }
    }
}