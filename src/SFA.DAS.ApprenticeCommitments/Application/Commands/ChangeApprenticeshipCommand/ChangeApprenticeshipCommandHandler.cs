using FluentValidation;
using MediatR;
using SFA.DAS.ApprenticeCommitments.Data;
using SFA.DAS.ApprenticeCommitments.Data.Models;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Application.Commands.ChangeApprenticeshipCommand
{
    public class ChangeApprenticeshipCommandValidator : AbstractValidator<ChangeApprenticeshipCommand>
    {
        public ChangeApprenticeshipCommandValidator()
        {
            RuleFor(model => model.ApprenticeshipId).Must(id => id > 0).WithMessage("The ApprenticeshipId must be positive");
            RuleFor(model => model.Email).NotNull().EmailAddress().WithMessage("Email must be a valid email address");
            RuleFor(model => model.EmployerAccountLegalEntityId).Must(id => id > 0).WithMessage("The EmployerAccountLegalEntityId must be positive");
            RuleFor(model => model.EmployerName).NotEmpty().WithMessage("The Employer Name is required");
            RuleFor(model => model.TrainingProviderId).Must(id => id > 0).WithMessage("The TrainingProviderId must be positive");
            RuleFor(model => model.TrainingProviderName).NotEmpty().WithMessage("The Training Provider Name is required");
        }
    }

    public class ChangeApprenticeshipCommandHandler : IRequestHandler<ChangeApprenticeshipCommand>
    {
        private readonly IApprenticeshipContext _statements;

        public ChangeApprenticeshipCommandHandler(IApprenticeshipContext statements) => _statements = statements;

        public async Task<Unit> Handle(ChangeApprenticeshipCommand command, CancellationToken cancellationToken)
        {
            var s = await _statements.FindByCommitmentsApprenticeshipId(command.ApprenticeshipId);

            var details = new ApprenticeshipDetails(
                command.EmployerAccountLegalEntityId,
                command.EmployerName,
                command.TrainingProviderId,
                command.TrainingProviderName,
                new CourseDetails(
                    command.CourseName,
                    command.CourseLevel,
                    command.CourseOption,
                    command.PlannedStartDate,
                    command.PlannedEndDate));

            s.RenewCommitment(details, command.ApprovedOn);

            return Unit.Value;
        }
    }
}