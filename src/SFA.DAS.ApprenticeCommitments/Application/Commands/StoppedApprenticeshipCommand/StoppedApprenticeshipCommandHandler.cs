using MediatR;
using SFA.DAS.ApprenticeCommitments.Data;
using SFA.DAS.ApprenticeCommitments.Data.Models;
using SFA.DAS.ApprenticeCommitments.Exceptions;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Application.Commands.StoppedApprenticeshipCommand
{
    public class ChangeRegistrationCommandHandler : IRequestHandler<StoppedApprenticeshipCommand>
    {
        private readonly IRevisionContext _revisions;

        public ChangeRegistrationCommandHandler(IRevisionContext _revisions) =>
            this._revisions = _revisions;

        public async Task<Unit> Handle(StoppedApprenticeshipCommand request, CancellationToken cancellationToken)
        {
            var apprenticeship =
                await _revisions.FindLatestByCommitmentsApprenticeshipId(request.CommitmentsApprenticeshipId)
                ?? throw new EntityNotFoundException(nameof(Revision), request.CommitmentsApprenticeshipId.ToString());

            apprenticeship.StoppedOn = request.CommitmentsStoppedOn;

            return Unit.Value;
        }
    }
}
