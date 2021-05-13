using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.ApprenticeCommitments.Data;
using SFA.DAS.ApprenticeCommitments.Data.Models;
using System.Threading;
using System.Threading.Tasks;

#nullable enable

namespace SFA.DAS.ApprenticeCommitments.Application.Commands.ChangeApprenticeshipCommand
{
    public class ChangeApprenticeshipCommandValidator : AbstractValidator<ChangeApprenticeshipCommand>
    {
        public ChangeApprenticeshipCommandValidator()
        {
            RuleFor(model => model.ApprenticeshipId).Must(id => id > 0).WithMessage("The ApprenticeshipId must be positive");
            //RuleFor(model => model.Email).NotNull().EmailAddress().WithMessage("Email must be a valid email address");
            RuleFor(model => model.EmployerAccountLegalEntityId).Must(id => id > 0).WithMessage("The EmployerAccountLegalEntityId must be positive");
            RuleFor(model => model.EmployerName).NotEmpty().WithMessage("The Employer Name is required");
            RuleFor(model => model.TrainingProviderId).Must(id => id > 0).WithMessage("The TrainingProviderId must be positive");
            RuleFor(model => model.TrainingProviderName).NotEmpty().WithMessage("The Training Provider Name is required");
        }
    }

    public class ChangeApprenticeshipCommandHandler : IRequestHandler<ChangeApprenticeshipCommand>
    {
        private readonly IApprenticeshipContext _statements;
        private readonly ILogger<ChangeApprenticeshipCommandHandler> _logger;

        public ChangeApprenticeshipCommandHandler(IApprenticeshipContext statements, ILogger<ChangeApprenticeshipCommandHandler> logger)
        {
            _statements = statements;
            _logger = logger;
        }

        public async Task<Unit> Handle(ChangeApprenticeshipCommand command, CancellationToken cancellationToken)
        {
            var existingStatement =
                await _statements.FindByCommitmentsApprenticeshipId(
                    command.ApprenticeshipId);

            if (existingStatement == null)
            {
                _logger.LogWarning("Ignoring update for missing apprenticeship {commitmentsApprenticeshipId}", command.ApprenticeshipId);
                return Unit.Value;
            }
            else
            {
                _logger.LogInformation("Updating apprenticeship {commitmentsApprenticeshipId}", command.ApprenticeshipId);

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

                existingStatement.RenewCommitment(details, command.ApprovedOn);

                return Unit.Value;
            }
        }
    }
}