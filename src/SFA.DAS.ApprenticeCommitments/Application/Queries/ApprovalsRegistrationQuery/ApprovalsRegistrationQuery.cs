using MediatR;

namespace SFA.DAS.ApprenticeCommitments.Application.Queries.ApprovalsRegistrationQuery
{
    public class ApprovalsRegistrationQuery : IRequest<ApprovalsRegistrationResponse>
    {
        public long CommitmentsApprenticeshipId { get; set; }
    }
}
