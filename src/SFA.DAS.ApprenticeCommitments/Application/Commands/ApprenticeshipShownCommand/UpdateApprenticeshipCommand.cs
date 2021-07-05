using MediatR;
using Microsoft.AspNetCore.JsonPatch;
using SFA.DAS.ApprenticeCommitments.Data;
using SFA.DAS.ApprenticeCommitments.Data.Models;
using SFA.DAS.ApprenticeCommitments.Infrastructure.Mediator;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Application.Commands.ApprenticeshipShownCommand
{
    public class UpdateApprenticeshipCommand : IUnitOfWorkCommand
    {
        public UpdateApprenticeshipCommand(Guid apprenticeId, long apprenticeshipId, JsonPatchDocument<Apprenticeship> updates)
        {
            ApprenticeId = apprenticeId;
            ApprenticeshipId = apprenticeshipId;
            Updates = updates;
        }

        public Guid ApprenticeId { get; }
        public long ApprenticeshipId { get; }
        public JsonPatchDocument<Apprenticeship> Updates { get; }
    }

    public class UpdateApprenticeshipCommandHandler : IRequestHandler<UpdateApprenticeshipCommand>
    {
        private readonly IApprenticeshipContext _apprenticeships;

        public UpdateApprenticeshipCommandHandler(IApprenticeshipContext apprenticeships)
        {
            _apprenticeships = apprenticeships;
        }

        public async Task<Unit> Handle(UpdateApprenticeshipCommand request, CancellationToken cancellationToken)
        {
            var app = await _apprenticeships.GetById(request.ApprenticeId, request.ApprenticeshipId);
            request.Updates.ApplyTo(app);
            return Unit.Value;
        }
    }
}