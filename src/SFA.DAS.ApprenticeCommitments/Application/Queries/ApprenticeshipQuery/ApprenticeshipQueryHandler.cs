using MediatR;
using SFA.DAS.ApprenticeCommitments.Data;
using SFA.DAS.ApprenticeCommitments.DTOs;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Application.Queries.ApprenticeshipQuery
{
    public class ApprenticeshipQueryHandler : IRequestHandler<ApprenticeshipQuery, ApprenticeshipDto>
    {
        private readonly ICommitmentStatementContext _apprenticeshipRepository;

        public ApprenticeshipQueryHandler(ICommitmentStatementContext apprenticeshipRepository)
            => _apprenticeshipRepository = apprenticeshipRepository;

        public async Task<ApprenticeshipDto> Handle(
            ApprenticeshipQuery request,
            CancellationToken cancellationToken)
        {
            var entity = await _apprenticeshipRepository
                .Find(request.ApprenticeId, request.ApprenticeshipId);

            return entity.MapToApprenticeshipDto();
        }
    }
}