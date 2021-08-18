using FluentValidation;
using SFA.DAS.ApprenticeCommitments.Data.Models;

namespace SFA.DAS.ApprenticeCommitments.Application.Commands.CreateApprenticeAccountCommand
{
    public class CreateApprenticeAccountCommandValidator : AbstractValidator<CreateApprenticeAccountCommand>
    {
        public CreateApprenticeAccountCommandValidator()
        {
            RuleFor(model => model.ApprenticeId).Must(id => id != default).WithMessage("The Apprentice Id must be valid");
            RuleFor(model => model.FirstName).NotNull().NotEmpty().WithMessage("FirstName is required");
            RuleFor(model => model.LastName).NotNull().NotEmpty().WithMessage("LastName is required");
            RuleFor(model => model.DateOfBirth).Must(dob => dob != default).WithMessage("The DateOfBirth must be valid");
            RuleFor(model => model.Email).NotNull().EmailAddress().WithMessage("Email must be a valid email address");
        }

        public class ApprenticeValidator : AbstractValidator<Apprentice>
        {
            public ApprenticeValidator()
            {
                RuleFor(model => model.FirstName).NotEmpty();
                RuleFor(model => model.LastName).NotEmpty();
                RuleFor(model => model.DateOfBirth).Must(dob => dob != default).WithMessage("'Date Of Birth' must be valid.");
                RuleFor(model => model.Email).Transform(e => e.ToString()).EmailAddress().WithMessage("Email must be a valid email address");
            }
        }
    }
}