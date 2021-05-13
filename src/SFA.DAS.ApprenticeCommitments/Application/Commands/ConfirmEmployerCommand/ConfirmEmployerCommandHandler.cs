using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.ApprenticeCommitments.Data;

namespace SFA.DAS.ApprenticeCommitments.Application.Commands.ConfirmEmployerCommand
{
    public class ConfirmEmployerCommandHandler
        : IRequestHandler<ConfirmEmployerCommand>
    {
        private readonly IApprenticeshipContext _apprenticeships;

        public ConfirmEmployerCommandHandler(IApprenticeshipContext apprenticeships)
            => _apprenticeships = apprenticeships;

        public async Task<Unit> Handle(ConfirmEmployerCommand request, CancellationToken cancellationToken)
        {
            var apprenticeship = await _apprenticeships.GetById(request.Id.ApprenticeId, request.Id.ApprenticeshipId);
            apprenticeship.ConfirmEmployer(request.Id.CommitmentStatementId, request.EmployerCorrect);
            return Unit.Value;
        }
    }
}