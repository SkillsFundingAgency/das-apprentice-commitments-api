using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.ApprenticeCommitments.Api.Types;
using SFA.DAS.ApprenticeCommitments.Application.Commands.RegistrationFirstSeenCommand;
using SFA.DAS.ApprenticeCommitments.Application.Commands.RegistrationReminderSentCommand;
using SFA.DAS.ApprenticeCommitments.Application.Commands.VerifyRegistrationCommand;
using SFA.DAS.ApprenticeCommitments.Application.Queries.RegistrationQuery;
using SFA.DAS.ApprenticeCommitments.Application.Queries.RegistrationRemindersQuery;

namespace SFA.DAS.ApprenticeCommitments.Api.Controllers
{
    [ApiController]
    public class RegistrationController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RegistrationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("registrations/{apprenticeId}")]
        public async Task<IActionResult> GetRegistration(Guid apprenticeId)
        {
            var response = await _mediator.Send(new RegistrationQuery { ApprenticeId = apprenticeId });

            if (response == null)
            {
                return NotFound();
            }
            return new OkObjectResult(response);
        }

        [HttpGet("registrations/reminders")]
        public async Task<IActionResult> GetRegistrationsNeedingReminders(DateTime invitationCutOffTime)
        {
            var response = await _mediator.Send(new RegistrationRemindersQuery { CutOffDateTime = invitationCutOffTime });

            return new OkObjectResult(response);
        }

        [HttpPost("registrations")]
        public async Task<IActionResult> VerifiedRegistration(VerifyRegistrationCommand command)
        {
            await _mediator.Send(command);

            return Ok();
        }

        [HttpPost("registrations/{apprenticeId}/reminder")]
        public async Task<IActionResult> RegistrationReminderSent(Guid apprenticeId, [FromBody] RegistrationReminderSentRequest request)
        {
            await _mediator.Send(new RegistrationReminderSentCommand( apprenticeId, request.SentOn));
            return Accepted();
        }

        [HttpPost("registrations/{apprenticeId}/firstseen")]
        public async Task<IActionResult> RegistrationFirstSeen(Guid apprenticeId, [FromBody] RegistrationFirstSeenRequest request)
        {
            await _mediator.Send(new RegistrationFirstSeenCommand(apprenticeId, request.SeenOn));
            return Accepted();
        }

    }
}