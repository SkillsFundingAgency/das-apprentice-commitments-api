using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

#nullable enable
[assembly: InternalsVisibleTo("SFA.DAS.ApprenticeCommitments.UnitTests")]
[assembly: InternalsVisibleTo("SFA.DAS.ApprenticeCommitments.Api.AcceptanceTests")]

namespace SFA.DAS.ApprenticeCommitments.Data.Models
{
    public class Apprenticeship
    {
        private Apprenticeship()
        {
        }

        public Apprenticeship(CommitmentStatement apprenticeship)
        {
            CommitmentStatements.Add(apprenticeship);
        }

        public long Id { get; private set; }

        public Apprentice Apprentice { get; private set; } = null!;

        public ICollection<CommitmentStatement> CommitmentStatements { get; private set; }
            = new List<CommitmentStatement>();

        public CommitmentStatement LatestCommitmentStatement
            => CommitmentStatements.OrderByDescending(x => x.CommitmentsApprovedOn).First();

        internal void ConfirmApprenticeship(bool apprenticeshipCorrect)
            => LatestCommitmentStatement.ConfirmApprenticeship(apprenticeshipCorrect);

        internal void ConfirmApprenticeshipDetails(bool apprenticeshipCorrect)
            => LatestCommitmentStatement.ConfirmApprenticeshipDetails(apprenticeshipCorrect);

        internal void ConfirmEmployer(bool apprenticeshipCorrect)
            => LatestCommitmentStatement.ConfirmEmployer(apprenticeshipCorrect);

        internal void ConfirmTrainingProvider(bool apprenticeshipCorrect)
            => LatestCommitmentStatement.ConfirmTrainingProvider(apprenticeshipCorrect);

        internal void ConfirmRolesAndResponsibilities(bool apprenticeshipCorrect)
            => LatestCommitmentStatement.ConfirmRolesAndResponsibilities(apprenticeshipCorrect);

        internal void ConfirmHowApprenticeshipWillBeDelivered(bool apprenticeshipCorrect)
            => LatestCommitmentStatement.ConfirmHowApprenticeshipWillBeDelivered(apprenticeshipCorrect);

        internal void RenewCommitment(long commitmentsApprenticeshipId, ApprenticeshipDetails details, DateTime approvedOn)
        {
            var newStatement = new CommitmentStatement(commitmentsApprenticeshipId, details, approvedOn); 
            newStatement.RenewedFromCommitment(LatestCommitmentStatement);
            CommitmentStatements.Add(newStatement);
        }
    }
}