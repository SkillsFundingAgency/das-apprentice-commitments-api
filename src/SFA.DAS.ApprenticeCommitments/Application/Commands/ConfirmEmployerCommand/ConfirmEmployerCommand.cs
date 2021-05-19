using System;
using SFA.DAS.ApprenticeCommitments.Infrastructure.Mediator;

#nullable enable

namespace SFA.DAS.ApprenticeCommitments.Application.Commands.ConfirmEmployerCommand
{

    public class ConfirmEmployerData
    {
        public bool EmployerCorrect { get; set; }
        public long ComitmentStatementId { get; set; }
    }


    public class ConfirmEmployerCommand : IUnitOfWorkCommand
    {
        public ConfirmEmployerCommand(
            Guid apprenticeId, long apprenticeshipId,
            ConfirmEmployerData confirmEmployerData)
        {
            ApprenticeId = apprenticeId;
            ApprenticeshipId = apprenticeshipId;
            confirmEmployerData = confirmEmployerData;
        }

        public Guid ApprenticeId { get; }
        public long ApprenticeshipId { get; }
        public ConfirmEmployerData ConfirmEmployerData { get; }
    }
}