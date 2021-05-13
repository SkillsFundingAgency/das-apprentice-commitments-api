using MediatR;
using SFA.DAS.ApprenticeCommitments.Data;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Application.Commands.ConfirmTrainingProviderCommand
{
    public class ConfirmTrainingProviderCommandHandler
        : IRequestHandler<ConfirmTrainingProviderCommand>
    {
        private readonly IApprenticeshipContext _apprenticeships;

        public ConfirmTrainingProviderCommandHandler(IApprenticeshipContext apprenticeships)
            => _apprenticeships = apprenticeships;

        public async Task<Unit> Handle(ConfirmTrainingProviderCommand request, CancellationToken cancellationToken)
        {
            var apprenticeship = await _apprenticeships.GetById(request.Id.ApprenticeId, request.Id.ApprenticeshipId);
            apprenticeship.ConfirmTrainingProvider(request.Id.CommitmentStatementId, request.TrainingProviderCorrect);
            return Unit.Value;
        }
    }
}