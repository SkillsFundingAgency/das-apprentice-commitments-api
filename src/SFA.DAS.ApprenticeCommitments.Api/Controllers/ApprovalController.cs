using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.ApprenticeCommitments.Application.Commands.ChangeRegistrationCommand;
using SFA.DAS.ApprenticeCommitments.Application.Commands.CreateRegistrationCommand;
using SFA.DAS.ApprenticeCommitments.Application.Commands.StoppedApprenticeshipCommand;
using System.Threading.Tasks;
using SFA.DAS.ApprenticeCommitments.Application.Queries.ApprovalsRegistrationQuery;

namespace SFA.DAS.ApprenticeCommitments.Api.Controllers
{
    [ApiController]
    public class ApprovalController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ApprovalController(IMediator mediator) => _mediator = mediator;

        [HttpPost("approvals")]
        public Task ApprenticeshipCreated(CreateRegistrationCommand command)
            => _mediator.Send(command);

        [HttpPut("approvals")]
        public Task ChangeOfCircumstances([FromBody] ChangeRegistrationCommand request)
            => _mediator.Send(request);

        [HttpPost("approvals/stopped")]
        public Task StoppedApprenticeship([FromBody] StoppedApprenticeshipCommand request)
            => _mediator.Send(request);

        [HttpGet("approvals/{commitmentsApprenticeshipId}/registration")]
        public async Task<IActionResult> GetApprovalsRegistration(long commitmentsApprenticeshipId)
        {

            var approvalsRegistration = await _mediator.Send(new ApprovalsRegistrationQuery
            {
                CommitmentsApprenticeshipId = commitmentsApprenticeshipId
            });

            if (approvalsRegistration == null)
            {
                return NotFound();
            }

            return Ok(approvalsRegistration);
        }
    }
}