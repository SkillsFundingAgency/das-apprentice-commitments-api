using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.ApprenticeCommitments.Application.Commands.ConfirmCommand;
using SFA.DAS.ApprenticeCommitments.Data.Models;
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

        [HttpPost("apprentices/{apprenticeId}/apprenticeships/{apprenticeshipId}/statements/{commitmentStatementId}/ApprenticeshipConfirmation")]
        public async Task ConfirmTrainingProvider(
            Guid apprenticeId, long apprenticeshipId, long commitmentStatementId,
            [FromBody] ConfirmApprenticeshipRequest request)
        {
            await _mediator.Send(new ConfirmCommand(
                apprenticeId, apprenticeshipId, commitmentStatementId,
                new Confirmations { ApprenticeshipCorrect = request.ApprenticeshipCorrect }));
        }
    }
}