using System;
using MediatR;
using SFA.DAS.ApprenticeCommitments.DTOs;

namespace SFA.DAS.ApprenticeCommitments.Application.Queries.MyApprenticeshipQuery
{
    public class MyApprenticeshipQuery : IRequest<ApprenticeshipDto?>
    {
        public MyApprenticeshipQuery(Guid apprenticeId, long apprenticeshipId)
        {
            ApprenticeId = apprenticeId;
            ApprenticeshipId = apprenticeshipId;
        }

        public Guid ApprenticeId { get; }
        public long ApprenticeshipId { get; }
    }
}