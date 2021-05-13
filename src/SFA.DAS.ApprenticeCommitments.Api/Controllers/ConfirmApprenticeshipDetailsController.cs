using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.ApprenticeCommitments.Application.Commands.ConfirmApprenticeshipDetailsCommand;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Api.Controllers
{
    public class ConfirmApprenticeshipDetailsRequest
    {
        public bool ApprenticeshipDetailsCorrect { get; set; }
    }

    [ApiController]
    public class ConfirmApprenticeshipDetailsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ConfirmApprenticeshipDetailsController(IMediator mediator) => _mediator = mediator;

        [HttpPost("apprentices/{apprenticeId}/apprenticeships/{apprenticeshipId}/apprenticeshipdetailsconfirmation")]
        [HttpPost("apprentices/{apprenticeId}/apprenticeships/{apprenticeshipId}/{commitmentStatementId}/ApprenticeshipDetailsConfirmation")]
        public async Task ConfirmTrainingProvider(
            Guid apprenticeId, long apprenticeshipId, long commitmentStatementId,
            [FromBody] ConfirmApprenticeshipDetailsRequest request)
        {
            await _mediator.Send(new ConfirmApprenticeshipDetailsCommand(
                (apprenticeId, apprenticeshipId, commitmentStatementId),
                request.ApprenticeshipDetailsCorrect));
        }
    }
}