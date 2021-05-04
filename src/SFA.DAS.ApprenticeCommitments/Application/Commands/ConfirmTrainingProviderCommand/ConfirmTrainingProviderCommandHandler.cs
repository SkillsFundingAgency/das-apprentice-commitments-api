using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.ApprenticeCommitments.Data;

namespace SFA.DAS.ApprenticeCommitments.Application.Commands.ConfirmTrainingProviderCommand
{
    public class ConfirmTrainingProviderCommandHandler
        : IRequestHandler<ConfirmTrainingProviderCommand>
    {
        private readonly ICommitmentStatementContext _apprenticeships;

        public ConfirmTrainingProviderCommandHandler(ICommitmentStatementContext apprenticeships)
            => _apprenticeships = apprenticeships;

        public async Task<Unit> Handle(ConfirmTrainingProviderCommand request, CancellationToken _)
        {
            var apprenticeship = await _apprenticeships.GetById(request.ApprenticeId, request.ApprenticeshipId);
            apprenticeship.ConfirmTrainingProvider(request.TrainingProviderCorrect);
            return Unit.Value;
        }
    }
}