using MediatR;
using SFA.DAS.ApprenticeCommitments.DTOs;

namespace SFA.DAS.ApprenticeCommitments.Application.Queries.ApprovalsRegistrationQuery
{
    public class ApprovalsRegistrationQuery : IRequest<RegistrationDto>
    {
        public long CommitmentsApprenticeshipId { get; set; }
    }
}
