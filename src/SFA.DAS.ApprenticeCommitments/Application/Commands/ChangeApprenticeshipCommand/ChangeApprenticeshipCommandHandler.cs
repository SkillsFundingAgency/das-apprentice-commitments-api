﻿using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.ApprenticeCommitments.Data;
using SFA.DAS.ApprenticeCommitments.Data.Models;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.ApprenticeCommitments.Exceptions;

#nullable enable

namespace SFA.DAS.ApprenticeCommitments.Application.Commands.ChangeApprenticeshipCommand
{
    public class ChangeApprenticeshipCommandHandler : IRequestHandler<ChangeApprenticeshipCommand>
    {
        private readonly IApprenticeshipContext _statements;
        private readonly IRegistrationContext _registrations;
        private readonly ILogger<ChangeApprenticeshipCommandHandler> _logger;

        public ChangeApprenticeshipCommandHandler(IApprenticeshipContext statements, IRegistrationContext registrations, ILogger<ChangeApprenticeshipCommandHandler> logger)
        {
            _statements = statements;
            _registrations = registrations;
            _logger = logger;
        }

        public async Task<Unit> Handle(ChangeApprenticeshipCommand command, CancellationToken cancellationToken)
        {
            var apprenticeshipId = command.CommitmentsContinuedApprenticeshipId ?? command.CommitmentsApprenticeshipId;

            var existingStatement = await _statements.FindByCommitmentsApprenticeshipId(apprenticeshipId);

            if (existingStatement == null)
            {
                _logger.LogWarning("No confirmed apprenticeship {apprenticeshipId} found", apprenticeshipId);
                await UpdateRegistration(command, apprenticeshipId);
            }
            else
            {
                _logger.LogInformation("Updating apprenticeship {apprenticeshipId}", apprenticeshipId);
                existingStatement.RenewCommitment(apprenticeshipId, BuildApprenticeshipDetails(command), command.CommitmentsApprovedOn);
            }

            return Unit.Value;
        }

        private async Task UpdateRegistration(ChangeApprenticeshipCommand command, long apprenticeshipId)
        {
            var registration = await _registrations.FindByCommitmentsApprenticeshipId(apprenticeshipId);

            if (registration == null)
            {
                _logger.LogError("A matching Registration record is expected but not found for commitments apprenticeship {apprenticeshipId}", apprenticeshipId);
                throw new DomainException($"No registration record found for commitments apprenticeship id {apprenticeshipId}");
            }

            _logger.LogInformation("Updating registration for apprenticeship {apprenticeshipId}", apprenticeshipId);
            registration.RenewApprenticeship(apprenticeshipId, command.CommitmentsApprovedOn, BuildApprenticeshipDetails(command));
        }

        private static ApprenticeshipDetails BuildApprenticeshipDetails(ChangeApprenticeshipCommand command)
        {
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
            return details;
        }

    }
}