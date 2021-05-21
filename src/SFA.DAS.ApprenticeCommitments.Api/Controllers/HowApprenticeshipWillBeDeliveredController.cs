using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.ApprenticeCommitments.Application.Commands.HowApprenticeshipWillBeDeliveredCommand;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Api.Controllers
{
    public class HowApprenticeshipWillBeDeliveredRequest
    {
        public bool HowApprenticeshipDeliveredCorrect { get; set; }
    }

    [ApiController]
    public class HowApprenticeshipWillBeDeliveredController : ControllerBase
    {
        private readonly IMediator _mediator;

        public HowApprenticeshipWillBeDeliveredController(IMediator mediator) => _mediator = mediator;

        [HttpPost("apprentices/{apprenticeId}/apprenticeships/{apprenticeshipId}/howapprenticeshipwillbedeliveredconfirmation")]
        public async Task<IActionResult> HowApprenticeshipWillBeDelivered(
            Guid apprenticeId, long apprenticeshipId,
            [FromBody] HowApprenticeshipWillBeDeliveredRequest request)
        {
            var command = new HowApprenticeshipWillBeDeliveredCommand(apprenticeId, apprenticeshipId, request.HowApprenticeshipDeliveredCorrect);
            await _mediator.Send(command);
            return Ok();
        }
    }
}