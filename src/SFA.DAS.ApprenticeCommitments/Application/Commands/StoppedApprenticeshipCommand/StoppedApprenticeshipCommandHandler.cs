using MediatR;
using SFA.DAS.ApprenticeCommitments.Data;
using SFA.DAS.ApprenticeCommitments.Infrastructure;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Application.Commands.StoppedApprenticeshipCommand
{
    public class ChangeRegistrationCommandHandler : IRequestHandler<StoppedApprenticeshipCommand>
    {
        private readonly IRegistrationContext _registrations;
        private readonly ITimeProvider _timeProvider;

        public ChangeRegistrationCommandHandler(IRegistrationContext revisions, ITimeProvider timeProvider) =>
            (_registrations, _timeProvider) = (revisions, timeProvider);

        public async Task<Unit> Handle(StoppedApprenticeshipCommand request, CancellationToken cancellationToken)
        {
            var apprenticeship = await _registrations
                .IncludeApprenticeships().GetByCommitmentsApprenticeshipId(request.CommitmentsApprenticeshipId);

            apprenticeship.StoppedReceivedOn = _timeProvider.Now;

            return Unit.Value;
        }
    }
}