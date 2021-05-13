using MediatR;
using SFA.DAS.ApprenticeCommitments.Application.Commands.ConfirmEmployerCommand;
using SFA.DAS.ApprenticeCommitments.Data;
using SFA.DAS.ApprenticeCommitments.Infrastructure.Mediator;
using System;
using System.Threading;
using System.Threading.Tasks;

#nullable enable

namespace SFA.DAS.ApprenticeCommitments.Application.Commands.ConfirmApprenticeshipDetailsCommand
{
    public class ConfirmApprenticeshipDetailsCommand : IUnitOfWorkCommand
    {
        public ConfirmApprenticeshipDetailsCommand(
            (Guid apprenticeId, long apprenticeshipId, long commitmentStatementId) id,
            bool apprenticeshipDetailsCorrect)
        {
            Id = new ApprenticeCommitmentStatementId(id);
            ApprenticeshipDetailsCorrect = apprenticeshipDetailsCorrect;
        }

        public ApprenticeCommitmentStatementId Id { get; }
        public bool ApprenticeshipDetailsCorrect { get; }
    }

    public class ConfirmApprenticeshipDetailsCommandHandler
        : IRequestHandler<ConfirmApprenticeshipDetailsCommand>
    {
        private readonly IApprenticeshipContext _apprenticeships;

        public ConfirmApprenticeshipDetailsCommandHandler(IApprenticeshipContext apprenticeships)
            => _apprenticeships = apprenticeships;

        public async Task<Unit> Handle(ConfirmApprenticeshipDetailsCommand request, CancellationToken cancellationToken)
        {
            var apprenticeship = await _apprenticeships.GetById(request.Id.ApprenticeId, request.Id.ApprenticeshipId);
            apprenticeship.ConfirmApprenticeshipDetails(request.Id.CommitmentStatementId, request.ApprenticeshipDetailsCorrect);
            return Unit.Value;
        }
    }
}