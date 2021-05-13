using MediatR;
using SFA.DAS.ApprenticeCommitments.Application.Commands.ConfirmEmployerCommand;
using SFA.DAS.ApprenticeCommitments.Data;
using SFA.DAS.ApprenticeCommitments.Infrastructure.Mediator;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Application.Commands.ConfirmApprenticeshipCommand
{
    public class ConfirmApprenticeshipCommand : IUnitOfWorkCommand
    {
        public ConfirmApprenticeshipCommand(
            (Guid apprenticeId, long apprenticeshipId, long commitmentStatementId) id,
            bool apprenticeshipCorrect)
        {
            Id = new ApprenticeCommitmentStatementId(id);
            ApprenticeshipCorrect = apprenticeshipCorrect;
        }

        public ApprenticeCommitmentStatementId Id { get; }
        public bool ApprenticeshipCorrect { get; }
    }

    public class ConfirmApprenticeshipCommandHandler
        : IRequestHandler<ConfirmApprenticeshipCommand>
    {
        private readonly IApprenticeshipContext _apprenticeships;

        public ConfirmApprenticeshipCommandHandler(IApprenticeshipContext apprenticeships)
            => _apprenticeships = apprenticeships;

        public async Task<Unit> Handle(ConfirmApprenticeshipCommand request, CancellationToken cancellationToken)
        {
            var apprenticeship = await _apprenticeships.GetById(request.Id.ApprenticeId, request.Id.ApprenticeshipId);
            apprenticeship.ConfirmApprenticeship(request.Id.CommitmentStatementId, request.ApprenticeshipCorrect);
            return Unit.Value;
        }
    }
}