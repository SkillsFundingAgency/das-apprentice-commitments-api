using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.ApprenticeCommitments.Application.Commands.ConfirmCommand;
using SFA.DAS.ApprenticeCommitments.Data.Models;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Api.Controllers
{
    public class ConfirmEmployerRequest
    {
        public bool EmployerCorrect { get; set; }
    }

    [ApiController]
    public class ConfirmEmployerController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ConfirmEmployerController(IMediator mediator) => _mediator = mediator;

        [HttpPost("apprentices/{apprenticeId}/apprenticeships/{apprenticeshipId}/EmployerConfirmation")]
        [HttpPost("apprentices/{apprenticeId}/apprenticeships/{apprenticeshipId}/{commitmentStatementId}/EmployerConfirmation")]
        public async Task ConfirmTrainingProvider(
            Guid apprenticeId, long apprenticeshipId, long commitmentStatementId,
            [FromBody] ConfirmEmployerRequest request)
        {
            await _mediator.Send(new ConfirmCommand(
                apprenticeId, apprenticeshipId, commitmentStatementId,
                new Confirmations { EmployerCorrect = request.EmployerCorrect }));
        }
    }
}