using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.ApprenticeCommitments.Data;

namespace SFA.DAS.ApprenticeCommitments.Application.Commands.ConfirmEmployerCommand
{
    public class ConfirmEmployerCommandHandler
        : IRequestHandler<ConfirmEmployerCommand>
    {
        private readonly ICommitmentStatementContext _apprenticeships;

        public ConfirmEmployerCommandHandler(ICommitmentStatementContext apprenticeships)
            => _apprenticeships = apprenticeships;

        public async Task<Unit> Handle(ConfirmEmployerCommand request, CancellationToken _)
        {
            var apprenticeship = await _apprenticeships.GetById(request.ApprenticeId, request.ApprenticeshipId);
            apprenticeship.ConfirmEmployer(request.EmployerCorrect);
            return Unit.Value;
        }
    }
}