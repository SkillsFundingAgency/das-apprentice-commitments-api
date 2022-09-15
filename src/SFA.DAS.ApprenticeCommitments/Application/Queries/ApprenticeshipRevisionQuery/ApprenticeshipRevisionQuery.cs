using MediatR;
using SFA.DAS.ApprenticeCommitments.DTOs;
using System;

namespace SFA.DAS.ApprenticeCommitments.Application.Queries.ApprenticeshipRevisionQuery
{
    public class ApprenticeshipRevisionQuery : IRequest<ApprenticeshipRevisionDto?>
    {
        public ApprenticeshipRevisionQuery(Guid apprenticeId, long apprenticeshipId, long revisionId)
        {
            ApprenticeId = apprenticeId;
            ApprenticeshipId = apprenticeshipId;
            RevisionId = revisionId;
        }

        public Guid ApprenticeId { get; }
        public long ApprenticeshipId { get; }
        public long RevisionId { get; }
    }
}