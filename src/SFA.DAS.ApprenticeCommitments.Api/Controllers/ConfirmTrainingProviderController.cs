﻿using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.ApprenticeCommitments.Application.Commands.ConfirmCommand;
using SFA.DAS.ApprenticeCommitments.Data.Models;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Api.Controllers
{
    public class ConfirmTrainingProviderRequest
    {
        public bool TrainingProviderCorrect { get; set; }
    }

    [ApiController]
    public class ConfirmTrainingProviderController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ConfirmTrainingProviderController(IMediator mediator) => _mediator = mediator;

        [HttpPost("apprentices/{apprenticeId}/apprenticeships/{apprenticeshipId}/revisions/{revisionId}/TrainingProviderConfirmation")]
        public async Task ConfirmTrainingProvider(
            Guid apprenticeId, long apprenticeshipId, long revisionId,
            [FromBody] ConfirmTrainingProviderRequest request)
        {
            await _mediator.Send(new ConfirmCommand(
                apprenticeId, apprenticeshipId, revisionId,
                new Confirmations { TrainingProviderCorrect = request.TrainingProviderCorrect }));
        }
    }
}