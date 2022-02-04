using MediatR;
using SFA.DAS.ApprenticeCommitments.Data;
using SFA.DAS.ApprenticeCommitments.Data.Models;
using SFA.DAS.ApprenticeCommitments.Exceptions;
using SFA.DAS.ApprenticeCommitments.Infrastructure;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Application.Commands.StoppedApprenticeshipCommand
{
    public class ChangeRegistrationCommandHandler : IRequestHandler<StoppedApprenticeshipCommand>
    {
        private readonly IRegistrationContext _registrations;
        private readonly IRevisionContext _revisions;
        private readonly ITimeProvider _timeProvider;

        public ChangeRegistrationCommandHandler(
            IRegistrationContext registrations, IRevisionContext revisions, ITimeProvider timeProvider) =>
            (_registrations, _revisions, _timeProvider) = (registrations, revisions, timeProvider);

        public async Task<Unit> Handle(StoppedApprenticeshipCommand request, CancellationToken cancellationToken)
        {
            var apprenticeship = await _revisions
                .FindLatestByCommitmentsApprenticeshipId(request.CommitmentsApprenticeshipId);

            if (apprenticeship != null)
            {
                apprenticeship.Stop(_timeProvider.Now);
                return Unit.Value;
            }

            var registration = await _registrations
                .IncludeApprenticeships()
                .FindByCommitmentsApprenticeshipId(request.CommitmentsApprenticeshipId);

            if (registration != null)
            {
                registration.Stop(_timeProvider.Now);
                return Unit.Value;
            }

            throw new EntityNotFoundException(
                nameof(Registration),
                request.CommitmentsApprenticeshipId.ToString());
        }
    }
}