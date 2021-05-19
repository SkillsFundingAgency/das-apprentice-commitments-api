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

        public async Task<Unit> Handle(ConfirmEmployerCommand command, CancellationToken _)
        {
            var apprenticeship = await _apprenticeships.GetById(command.ApprenticeId, command.ApprenticeshipId);
            apprenticeship.ConfirmEmployer(command.ConfirmEmployerData);
            return Unit.Value;
        }
    }
}