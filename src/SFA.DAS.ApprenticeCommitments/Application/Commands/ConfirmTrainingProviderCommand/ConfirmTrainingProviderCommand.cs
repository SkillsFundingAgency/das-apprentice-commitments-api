using SFA.DAS.ApprenticeCommitments.Application.Commands.ConfirmEmployerCommand;
using SFA.DAS.ApprenticeCommitments.Infrastructure.Mediator;
using System;

#nullable enable

namespace SFA.DAS.ApprenticeCommitments.Application.Commands.ConfirmTrainingProviderCommand
{
    public class ConfirmTrainingProviderCommand : IUnitOfWorkCommand
    {
        public ConfirmTrainingProviderCommand(
            (Guid apprenticeId, long apprenticeshipId, long commitmentStatementId) id,
            bool trainingProviderCorrect)
        {
            Id = new ApprenticeCommitmentStatementId(id);
            TrainingProviderCorrect = trainingProviderCorrect;
        }

        public ApprenticeCommitmentStatementId Id { get; }
        public bool TrainingProviderCorrect { get; }
    }
}