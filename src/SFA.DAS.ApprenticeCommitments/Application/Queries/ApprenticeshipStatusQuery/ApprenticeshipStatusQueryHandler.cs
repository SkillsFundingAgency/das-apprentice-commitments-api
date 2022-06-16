using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.ApprenticeCommitments.Data;
using SFA.DAS.ApprenticeCommitments.DTOs;

namespace SFA.DAS.ApprenticeCommitments.Application.Queries.ApprenticeshipStatusQuery
{
    public class ApprenticeshipStatusQueryHandler : IRequestHandler<ApprenticeshipStatusQuery, ApprenticeshipStatusDto?>
    {
        private readonly IApprenticeshipContext _apprenticeshipRepository;

        public ApprenticeshipStatusQueryHandler(IApprenticeshipContext apprenticeshipRepository)
            => _apprenticeshipRepository = apprenticeshipRepository;

        public async Task<ApprenticeshipStatusDto?> Handle(ApprenticeshipStatusQuery request, CancellationToken cancellationToken)
        {
            var apprenticeship = (await _apprenticeshipRepository.FindForApprentice(request.ApprenticeId, request.ApprenticeshipId));

            if (apprenticeship == null)
                return null;

            return new ApprenticeshipStatusDto
            {
                ApprenticeshipId = request.ApprenticeshipId,
                HasBeenConfirmedAtLeastOnce = apprenticeship.ApprenticeshipHasPreviouslyBeenConfirmed,
            };
        }
    }
}