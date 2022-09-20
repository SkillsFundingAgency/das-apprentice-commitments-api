using FluentValidation;

namespace SFA.DAS.ApprenticeCommitments.Application.Queries.RegistrationQuery
{
    public class RegistrationQueryValidator : AbstractValidator<RegistrationQuery>
    {
        public RegistrationQueryValidator()
        {
            RuleFor(model => model.RegistrationId).Must(id => id != default).WithMessage("The Registration Id must be valid");
        }
    }
}
