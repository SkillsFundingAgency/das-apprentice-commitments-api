using MediatR;
using SFA.DAS.ApprenticeCommitments.Data;
using SFA.DAS.ApprenticeCommitments.Infrastructure.Mediator;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Application.Commands.ConfirmApprenticeshipCommand
{
    public class ConfirmApprenticeshipCommand : IUnitOfWorkCommand
    {
        public ConfirmApprenticeshipCommand(Guid apprenticeId, long apprenticeshipId, bool apprenticeshipCorrect)
        {
            ApprenticeId = apprenticeId;
            ApprenticeshipId = apprenticeshipId;
            ApprenticeshipCorrect = apprenticeshipCorrect;
        }

        public Guid ApprenticeId { get; }
        public long ApprenticeshipId { get; }
        public bool ApprenticeshipCorrect { get; }
    }

    public class ConfirmApprenticeshipCommandHandler
        : IRequestHandler<ConfirmApprenticeshipCommand>
    {
        private readonly IApprenticeshipContext _apprenticeships;

        public ConfirmApprenticeshipCommandHandler(IApprenticeshipContext apprenticeships)
            => _apprenticeships = apprenticeships;

        public async Task<Unit> Handle(ConfirmApprenticeshipCommand request, CancellationToken _)
        {
            var apprenticeship = await _apprenticeships.GetById(request.ApprenticeId, request.ApprenticeshipId);
            apprenticeship.ConfirmApprenticeship(request.ApprenticeshipCorrect);
            return Unit.Value;
        }
    }
}