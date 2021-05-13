using MediatR;
using SFA.DAS.ApprenticeCommitments.Application.Commands.ConfirmEmployerCommand;
using SFA.DAS.ApprenticeCommitments.Data;
using SFA.DAS.ApprenticeCommitments.Infrastructure.Mediator;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Application.Commands.ConfirmRolesAndResponsibilitiesCommand
{
    public class ConfirmRolesAndResponsibilitiesCommand : IUnitOfWorkCommand
    {
        public ConfirmRolesAndResponsibilitiesCommand(
            (Guid apprenticeId, long apprenticeshipId, long commitmentStatementId) id,
            bool rolesAndResponsibilitiesCorrect)
        {
            Id = new ApprenticeCommitmentStatementId(id);
            RolesAndResponsibilitiesCorrect = rolesAndResponsibilitiesCorrect;
        }

        public ApprenticeCommitmentStatementId Id { get; }
        public bool RolesAndResponsibilitiesCorrect { get; }
    }

    public class ConfirmRolesAndResponsibilitiesCommandHandler
        : IRequestHandler<ConfirmRolesAndResponsibilitiesCommand>
    {
        private readonly IApprenticeshipContext _apprenticeships;

        public ConfirmRolesAndResponsibilitiesCommandHandler(IApprenticeshipContext apprenticeships)
            => _apprenticeships = apprenticeships;

        public async Task<Unit> Handle(ConfirmRolesAndResponsibilitiesCommand request, CancellationToken cancellationToken)
        {
            var apprenticeship = await _apprenticeships.GetById(request.Id.ApprenticeId, request.Id.ApprenticeshipId);
            apprenticeship.ConfirmRolesAndResponsibilities(request.Id.CommitmentStatementId, request.RolesAndResponsibilitiesCorrect);
            return Unit.Value;
        }
    }
}