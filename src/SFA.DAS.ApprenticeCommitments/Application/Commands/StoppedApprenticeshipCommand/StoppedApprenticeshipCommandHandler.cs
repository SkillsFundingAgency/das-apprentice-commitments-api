using MediatR;
using SFA.DAS.ApprenticeCommitments.Data;
using SFA.DAS.ApprenticeCommitments.Data.Models;
using SFA.DAS.ApprenticeCommitments.Exceptions;
using SFA.DAS.ApprenticeCommitments.Infrastructure;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Application.Commands.StoppedApprenticeshipCommand
{
    public class StoppedApprenticeshipCommandHandler : IRequestHandler<StoppedApprenticeshipCommand>
    {
        private readonly IRegistrationContext _registrations;
        private readonly IRevisionContext _revisions;
        private readonly ITimeProvider _timeProvider;

        public StoppedApprenticeshipCommandHandler(
            IRegistrationContext registrations, IRevisionContext revisions, ITimeProvider timeProvider) =>
            (_registrations, _revisions, _timeProvider) = (registrations, revisions, timeProvider);

        public async Task<Unit> Handle(StoppedApprenticeshipCommand request, CancellationToken cancellationToken)
        {
            var apprenticeship
                = await FindRevision(request)
                ?? await FindRegistration(request)
                ?? throw NotFound(request);

            apprenticeship.Stop(_timeProvider.Now);

            return Unit.Value;
        }

        private async Task<IStoppable?> FindRevision(StoppedApprenticeshipCommand request)
            => await _revisions
                .FindLatestByCommitmentsApprenticeshipId(request.CommitmentsApprenticeshipId);

        private async Task<IStoppable?> FindRegistration(StoppedApprenticeshipCommand request)
            => await _registrations
                .IncludeApprenticeships()
                .FindByCommitmentsApprenticeshipId(request.CommitmentsApprenticeshipId);

        private static Exception NotFound(StoppedApprenticeshipCommand request)
            => new EntityNotFoundException(
                nameof(Registration),
                request.CommitmentsApprenticeshipId.ToString());
    }
}