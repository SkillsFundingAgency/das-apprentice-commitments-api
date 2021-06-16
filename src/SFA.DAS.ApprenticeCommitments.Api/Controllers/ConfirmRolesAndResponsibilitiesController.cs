﻿using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.ApprenticeCommitments.Application.Commands.ConfirmCommand;
using SFA.DAS.ApprenticeCommitments.Data.Models;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Api.Controllers
{
    public class ConfirmRolesAndResponsibilitiesRequest
    {
        public bool RolesAndResponsibilitiesCorrect { get; set; }
    }

    [ApiController]
    public class ConfirmRolesAndResponsibilitiesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ConfirmRolesAndResponsibilitiesController(IMediator mediator) => _mediator = mediator;

        [HttpPost("apprentices/{apprenticeId}/apprenticeships/{apprenticeshipId}/statements/{commitmentStatementId}/RolesAndResponsibilitiesConfirmation")]
        public async Task ConfirmTrainingProvider(
            Guid apprenticeId, long apprenticeshipId, long commitmentStatementId,
            [FromBody] ConfirmRolesAndResponsibilitiesRequest request)
        {
            await _mediator.Send(new ConfirmCommand(
                apprenticeId, apprenticeshipId, commitmentStatementId,
                new Confirmations { RolesAndResponsibilitiesCorrect = request.RolesAndResponsibilitiesCorrect }));
        }
    }
}