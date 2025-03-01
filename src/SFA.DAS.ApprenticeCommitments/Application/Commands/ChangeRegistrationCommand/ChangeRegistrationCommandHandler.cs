﻿using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.ApprenticeCommitments.Data;
using SFA.DAS.ApprenticeCommitments.Data.Models;
using System;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;

#nullable enable

namespace SFA.DAS.ApprenticeCommitments.Application.Commands.ChangeRegistrationCommand
{
    public class ChangeRegistrationCommandHandler : IRequestHandler<ChangeRegistrationCommand>
    {
        private readonly IApprenticeshipContext _apprenticeships;
        private readonly IRegistrationContext _registrations;
        private readonly ILogger<ChangeRegistrationCommandHandler> _logger;

        public ChangeRegistrationCommandHandler(IApprenticeshipContext apprenticeships, IRegistrationContext registrations, ILogger<ChangeRegistrationCommandHandler> logger)
        {
            _apprenticeships = apprenticeships;
            _registrations = registrations;
            _logger = logger;
        }


        private async Task UpdateOrCreateRegistration(ChangeRegistrationCommand command, long apprenticeshipId)
        {
            var registration = await _registrations.FindByCommitmentsApprenticeshipId(apprenticeshipId);

            if (registration == null)
            {
                _logger.LogInformation("Adding registration for apprenticeship {apprenticeshipId} because it didn't exist", apprenticeshipId);
                await _registrations.AddAsync(
                    new Registration(Guid.NewGuid(), command.CommitmentsApprenticeshipId,
                        command.CommitmentsApprovedOn,
                        BuildPersonalDetails(command),
                        BuildApprenticeshipDetails(command)));
            }
            else
            {
                _logger.LogInformation("Updating registration for apprenticeship {apprenticeshipId}", apprenticeshipId);
                registration.RenewApprenticeship(command.CommitmentsApprenticeshipId, command.CommitmentsApprovedOn,
                    BuildApprenticeshipDetails(command), BuildPersonalDetails(command));
            }
        }

        private static PersonalInformation BuildPersonalDetails(ChangeRegistrationCommand command)
            => new PersonalInformation(
                command.FirstName,
                command.LastName,
                command.DateOfBirth,
                new MailAddress(command.Email));

        private static ApprenticeshipDetails BuildApprenticeshipDetails(ChangeRegistrationCommand command)
        {
            var details = new ApprenticeshipDetails(
                command.EmployerAccountLegalEntityId,
                command.EmployerName,
                command.TrainingProviderId,
                command.TrainingProviderName,
                command.DeliveryModel,
                new RplDetails(command.RecognisePriorLearning,
                    command.DurationReducedByHours,
                    command.DurationReducedBy),
                new CourseDetails(
                    command.CourseName,
                    command.CourseLevel,
                    command.CourseOption,
                    command.PlannedStartDate,
                    command.PlannedEndDate,
                    command.CourseDuration,
                    command.EmploymentEndDate));
            return details;
        }

        async Task IRequestHandler<ChangeRegistrationCommand>.Handle(ChangeRegistrationCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("ChangeRegistrationCommand for Approval {Approval} (continuning {PreviousApproval})", request.CommitmentsApprenticeshipId, request.CommitmentsContinuedApprenticeshipId);

            var apprenticeshipId = request.CommitmentsContinuedApprenticeshipId ?? request.CommitmentsApprenticeshipId;

            var apprenticeship = await _apprenticeships.FindByCommitmentsApprenticeshipId(apprenticeshipId);

            if (apprenticeship == null)
            {
                _logger.LogWarning("No confirmed apprenticeship {apprenticeshipId} found", apprenticeshipId);
                await UpdateOrCreateRegistration(request, apprenticeshipId);
            }
            else
            {
                _logger.LogInformation("Updating apprenticeship {apprenticeshipId}", apprenticeshipId);
                apprenticeship.Revise(request.CommitmentsApprenticeshipId, BuildApprenticeshipDetails(request), request.CommitmentsApprovedOn);
            }
        }
    }
}