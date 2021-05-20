using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.ApprenticeCommitments.Data;

namespace SFA.DAS.ApprenticeCommitments.Application.Commands.ConfirmCommand
{
    public class ConfirmCommandHandler
        : IRequestHandler<ConfirmCommand>
    {
        private readonly IApprenticeshipContext _apprenticeships;

        public ConfirmCommandHandler(IApprenticeshipContext apprenticeships)
            => _apprenticeships = apprenticeships;

        public async Task<Unit> Handle(ConfirmCommand request, CancellationToken cancellationToken)
        {
            var apprenticeship = await _apprenticeships.GetById(request.ApprenticeId, request.ApprenticeshipId);
            apprenticeship.Confirm(request.CommitmentStatementId, request.Confirmations);
            return Unit.Value;
        }
    }
}