using MediatR;
using Microsoft.Extensions.Logging;
using NServiceBus;
using SFA.DAS.ApprenticeCommitments.Data;
using SFA.DAS.ApprenticeCommitments.Data.Models;
using SFA.DAS.ApprenticeCommitments.Exceptions;
using SFA.DAS.ApprenticeCommitments.Messages.Events;
using System;
using System.Threading;
using System.Threading.Tasks;

#nullable enable

namespace SFA.DAS.ApprenticeCommitments.Application.Commands.ChangeApprenticeshipCommand
{
    public class ChangeApprenticeshipCommandHandler : IRequestHandler<ChangeApprenticeshipCommand>
    {
        private readonly IApprenticeshipContext _statements;
        private readonly IRegistrationContext _registrations;
        private readonly ILogger<ChangeApprenticeshipCommandHandler> _logger;
        private readonly IMessageSession _messageSession;

        public ChangeApprenticeshipCommandHandler(IApprenticeshipContext statements, IRegistrationContext registrations, IMessageSession session, ILogger<ChangeApprenticeshipCommandHandler> logger)
        {
            _statements = statements;
            _registrations = registrations;
            _logger = logger;
            _messageSession = session;
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
                var e = existingStatement.RenewCommitment(command.CommitmentsApprenticeshipId, BuildApprenticeshipDetails(command), command.CommitmentsApprovedOn);

                if (e != null) await _messageSession.Publish(e);
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
            registration.RenewApprenticeship(command.CommitmentsApprenticeshipId, command.CommitmentsApprovedOn, BuildApprenticeshipDetails(command));
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
