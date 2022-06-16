using System;
using MediatR;
using SFA.DAS.ApprenticeCommitments.DTOs;

namespace SFA.DAS.ApprenticeCommitments.Application.Queries.ApprenticeshipStatusQuery
{
    public class ApprenticeshipStatusQuery : IRequest<ApprenticeshipStatusDto?>
    {
        public ApprenticeshipStatusQuery(Guid apprenticeId, long apprenticeshipId)
        {
            ApprenticeId = apprenticeId;
            ApprenticeshipId = apprenticeshipId;
        }

        public Guid ApprenticeId { get; }
        public long ApprenticeshipId { get; }
    }
}