using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.ApprenticeCommitments.Data;
using SFA.DAS.ApprenticeCommitments.DTOs;

#nullable enable

namespace SFA.DAS.ApprenticeCommitments.Application.Queries.MyApprenticeshipQuery
{
    public class MyApprenticeshipQueryHandler : IRequestHandler<MyApprenticeshipQuery, ApprenticeshipDto?>
    {
        private readonly IApprenticeshipContext _apprenticeshipRepository;

        public MyApprenticeshipQueryHandler(IApprenticeshipContext apprenticeshipRepository)
        {
            _apprenticeshipRepository = apprenticeshipRepository;
        }

        public async Task<ApprenticeshipDto?> Handle(
            MyApprenticeshipQuery request,
            CancellationToken cancellationToken)
        {
            var entity = await _apprenticeshipRepository
                .FindForApprentice(request.ApprenticeId, request.ApprenticeshipId);

            return entity?.MapToMyApprenticeshipDto();
        }
    }
}