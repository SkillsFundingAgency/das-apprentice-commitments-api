using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.ApprenticeCommitments.Data;
using SFA.DAS.ApprenticeCommitments.Data.Models;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Application.Commands.CreateRegistrationCommand
{
    public class CreateRegistrationCommandHandler : IRequestHandler<CreateRegistrationCommand>
    {
        private readonly IRegistrationContext _registrations;
        private readonly ILogger<CreateRegistrationCommandHandler> _logger;

        public CreateRegistrationCommandHandler(IRegistrationContext registrations, ILogger<CreateRegistrationCommandHandler> logger)
            => (_registrations, _logger) = (registrations, logger);

        public async Task<Unit> Handle(CreateRegistrationCommand request, CancellationToken cancellationToken)
        {
            var exists = await _registrations.FindByCommitmentsApprenticeshipId(request.CommitmentsApprenticeshipId);
            if (exists != null)
            {
                _logger.LogInformation("Registration with commitmentsApprenticeshipID {commitmentsApprenticeshipId} already exists", request.CommitmentsApprenticeshipId);
                return Unit.Value;
            }

            await _registrations.AddAsync(new Registration(
                request.RegistrationId,
                request.CommitmentsApprenticeshipId,
                request.CommitmentsApprovedOn,
                new PersonalInformation(
                    request.FirstName,
                    request.LastName,
                    request.DateOfBirth,
                    new MailAddress(request.Email)),
                new ApprenticeshipDetails(
                    request.EmployerAccountLegalEntityId,
                    request.EmployerName,
                    request.TrainingProviderId,
                    request.TrainingProviderName,
                    request.DeliveryModel,
                    request.DurationReducedByHours,
                    request.DurationReducedBy,
                    new CourseDetails(
                        request.CourseName,
                        request.CourseLevel,
                        request.CourseOption,
                        request.PlannedStartDate,
                        request.PlannedEndDate,
                        request.CourseDuration,
                        request.EmploymentEndDate))));

            return Unit.Value;
        }
    }
}