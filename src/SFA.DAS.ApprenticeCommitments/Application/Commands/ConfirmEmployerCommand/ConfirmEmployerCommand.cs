using SFA.DAS.ApprenticeCommitments.Infrastructure.Mediator;
using System;

#nullable enable

namespace SFA.DAS.ApprenticeCommitments.Application.Commands.ConfirmEmployerCommand
{
    public class ApprenticeCommitmentStatementId
    {
        public ApprenticeCommitmentStatementId((Guid apprenticeId, long apprenticeshipId, long commitmentStatementId) id)
        {
            ApprenticeId = id.apprenticeId;
            ApprenticeshipId = id.apprenticeshipId;
            CommitmentStatementId = id.commitmentStatementId;
        }

        public Guid ApprenticeId { get; }
        public long ApprenticeshipId { get; }
        public long CommitmentStatementId { get; }
    }

    public class ConfirmEmployerCommand : IUnitOfWorkCommand
    {
        public ConfirmEmployerCommand(
            (Guid apprenticeId, long apprenticeshipId, long commitmentStatementId) id,
            bool trainingProviderCorrect)
        {
            Id = new ApprenticeCommitmentStatementId(id);
            EmployerCorrect = trainingProviderCorrect;
        }

        public ApprenticeCommitmentStatementId Id { get; }
        public bool EmployerCorrect { get; }
    }
}