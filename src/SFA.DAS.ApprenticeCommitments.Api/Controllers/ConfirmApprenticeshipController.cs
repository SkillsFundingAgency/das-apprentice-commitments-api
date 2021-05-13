using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.ApprenticeCommitments.Application.Commands.ConfirmApprenticeshipCommand;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Api.Controllers
{
    public class ConfirmApprenticeshipRequest
    {
        public bool ApprenticeshipCorrect { get; set; }
    }

    public class ConfirmApprenticeshipController : Controller
    {
        private readonly IMediator _mediator;

        public ConfirmApprenticeshipController(IMediator mediator) => _mediator = mediator;

        [HttpPost("apprentices/{apprenticeId}/apprenticeships/{apprenticeshipId}/ApprenticeshipConfirmation")]
        [HttpPost("apprentices/{apprenticeId}/apprenticeships/{apprenticeshipId}/{commitmentStatementId}/ApprenticeshipConfirmation")]
        public async Task ConfirmTrainingProvider(
            Guid apprenticeId, long apprenticeshipId, long commitmentStatementId,
            [FromBody] ConfirmApprenticeshipRequest request)
        {
            await _mediator.Send(new ConfirmApprenticeshipCommand(
                (apprenticeId, apprenticeshipId, commitmentStatementId),
                request.ApprenticeshipCorrect));
        }
    }
}
