using FluentValidation;

namespace SFA.DAS.ApprenticeCommitments.Application.Queries.ApprovalsRegistrationQuery
{
    public class ApprovalsRegistrationValidator : AbstractValidator<ApprovalsRegistrationQuery>
    {
        public ApprovalsRegistrationValidator()
        {
            RuleFor(model => model.CommitmentsApprenticeshipId).Must(id => id != default).WithMessage("The approvals apprenticeship identity must be valid");
        }
    }
}
