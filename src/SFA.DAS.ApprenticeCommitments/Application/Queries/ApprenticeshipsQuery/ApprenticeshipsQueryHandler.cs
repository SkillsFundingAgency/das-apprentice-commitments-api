using MediatR;
using SFA.DAS.ApprenticeCommitments.Data;
using SFA.DAS.ApprenticeCommitments.Data.Models;
using SFA.DAS.ApprenticeCommitments.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

#nullable enable

namespace SFA.DAS.ApprenticeCommitments.Application.Queries.ApprenticeshipsQuery
{
    public class ApprenticeshipsQueryHandler
        : IRequestHandler<ApprenticeshipsQuery, List<ApprenticeshipDto>>
    {
        private readonly IApprenticeshipContext _apprenticeshipRepository;

        public ApprenticeshipsQueryHandler(IApprenticeshipContext apprenticeshipRepository)
            => _apprenticeshipRepository = apprenticeshipRepository;

        public async Task<List<ApprenticeshipDto>> Handle(
            ApprenticeshipsQuery request,
            CancellationToken cancellationToken)
        {
            List<Apprenticeship> apprenticeships = await _apprenticeshipRepository
                .FindAllForApprentice(request.ApprenticeId);

            return apprenticeships.SelectMany(x => x.CommitmentStatements)
                .Select(x => x.MapToApprenticeshipDto()!)
                .ToList();
        }
    }
}