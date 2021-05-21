using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.ApprenticeCommitments.Data;

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
            var apprenticeship = await _apprenticeships.GetById(request.ApprenticeId, request.ApprenticeshipId);
            apprenticeship.ConfirmTrainingProvider(request.TrainingProviderCorrect);
            return Unit.Value;
        }
    }
}