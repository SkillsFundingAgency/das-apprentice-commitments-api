using MediatR;
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
            Guid apprenticeId, long apprenticeshipId,
            bool rolesAndResponsibilitiesCorrect)
        {
            ApprenticeId = apprenticeId;
            ApprenticeshipId = apprenticeshipId;
            RolesAndResponsibilitiesCorrect = rolesAndResponsibilitiesCorrect;
        }

        public Guid ApprenticeId { get; }
        public long ApprenticeshipId { get; }
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
            var apprenticeship = await _apprenticeships.GetById(request.ApprenticeId, request.ApprenticeshipId);
            apprenticeship.ConfirmRolesAndResponsibilities(request.RolesAndResponsibilitiesCorrect);
            return Unit.Value;
        }
    }
}
