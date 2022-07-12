using MediatR;
using SFA.DAS.ApprenticeCommitments.Data;
using SFA.DAS.ApprenticeCommitments.DTOs;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Application.Queries.ApprenticeshipRevisionQuery
{
    public class ApprenticeshipRevisionQueryHandler : IRequestHandler<ApprenticeshipRevisionQuery, ApprenticeshipRevisionDto?>
    {
        private readonly IRevisionContext _revisionRepository;

        public ApprenticeshipRevisionQueryHandler(IRevisionContext revisionRepository)
            => _revisionRepository = revisionRepository;

        public async Task<ApprenticeshipRevisionDto?> Handle(
            ApprenticeshipRevisionQuery request,
            CancellationToken cancellationToken)
        {
            var entity = await _revisionRepository
                .GetById(request.ApprenticeId, request.ApprenticeshipId, request.RevisionId);

            return entity?.MapToApprenticeshipRevisionDto();
        }
    }
}