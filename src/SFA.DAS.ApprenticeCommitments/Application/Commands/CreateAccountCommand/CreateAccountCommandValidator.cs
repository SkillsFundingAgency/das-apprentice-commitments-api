using FluentValidation;

namespace SFA.DAS.ApprenticeCommitments.Application.Commands.CreateAccountCommand
{
    public class CreateAccountCommandValidator : AbstractValidator<CreateAccountCommand>
    {
        public CreateAccountCommandValidator()
        {
            RuleFor(model => model.ApprenticeId).Must(id => id != default).WithMessage("The Apprentice Id must be valid");
            RuleFor(model => model.FirstName).NotNull().NotEmpty().WithMessage("FirstName is required");
            RuleFor(model => model.LastName).NotNull().NotEmpty().WithMessage("LastName is required");
            RuleFor(model => model.DateOfBirth).Must(dob => dob != default).WithMessage("The DateOfBirth must be valid");
            RuleFor(model => model.Email).NotNull().EmailAddress().WithMessage("Email must be a valid email address");
        }
    }
}
