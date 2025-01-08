using MediatR;
using SFA.DAS.ApprenticeCommitments.Data;
using SFA.DAS.ApprenticeCommitments.Data.Models;
using SFA.DAS.ApprenticeCommitments.Infrastructure;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.ApprenticeCommitments.Application.Commands.StoppedApprenticeshipCommand
{
    public class StoppedApprenticeshipCommandHandler : IRequestHandler<StoppedApprenticeshipCommand>
    {
        private readonly IRegistrationContext _registrations;
        private readonly IRevisionContext _revisions;
        private readonly ITimeProvider _timeProvider;
        private readonly ILogger<StoppedApprenticeshipCommandHandler> _logger;

        public StoppedApprenticeshipCommandHandler(
            IRegistrationContext registrations, IRevisionContext revisions, ITimeProvider timeProvider, ILogger<StoppedApprenticeshipCommandHandler> logger) =>
            (_registrations, _revisions, _timeProvider, _logger) = (registrations, revisions, timeProvider, logger);


        private async Task<IStoppable?> FindRevision(StoppedApprenticeshipCommand request)
            => await _revisions
                .FindLatestByCommitmentsApprenticeshipId(request.CommitmentsApprenticeshipId);

        private async Task<IStoppable?> FindRegistration(StoppedApprenticeshipCommand request)
            => await _registrations
                .IncludeApprenticeships()
                .FindByCommitmentsApprenticeshipId(request.CommitmentsApprenticeshipId);

        async Task IRequestHandler<StoppedApprenticeshipCommand>.Handle(StoppedApprenticeshipCommand request, CancellationToken cancellationToken)
        {
            var apprenticeship
                = await FindRevision(request)
                ?? await FindRegistration(request);

            if (apprenticeship == null)
            {
                _logger.LogInformation("No apprenticeship details found for {commitmentsApprenticeshipId} which has been stopped", request.CommitmentsApprenticeshipId);
            }
            else
            {
                apprenticeship.Stop(_timeProvider.Now);
            }
        }
    }
}