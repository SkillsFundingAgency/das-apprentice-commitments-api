using MediatR;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.ApprenticeCommitments.Application.Commands.CreateApprenticeAccountCommand;
using SFA.DAS.ApprenticeCommitments.Application.Commands.UpdateApprenticeCommand;
using SFA.DAS.ApprenticeCommitments.Application.Queries.ApprenticesQuery;
using SFA.DAS.ApprenticeCommitments.DTOs;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Api.Controllers
{
    [ApiController]
    [Obsolete]
    public class ApprenticesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ApprenticesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("apprentices/{id}")]
        public async Task<IActionResult> GetApprentice(Guid id)
        {
            var result = await _mediator.Send(new ApprenticesQuery(id));
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost("apprentices")]
        public async Task PostApprentice(CreateApprenticeAccountCommand command)
            => await _mediator.Send(command);

        [HttpPatch("apprentices/{id}")]
        public async Task UpdateApprentice(Guid id, JsonPatchDocument<ApprenticePatchDto> changes)
            => await _mediator.Send(new UpdateApprenticeCommand(id, changes));
    }
}