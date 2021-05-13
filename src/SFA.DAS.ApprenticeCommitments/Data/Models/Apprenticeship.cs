using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable

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
            => CommitmentStatements.OrderByDescending(x => x.CommitmentsApprovedOn).FirstOrDefault();

        private CommitmentStatement GetCommitmentStatement(long commitmentStatementId)
            => commitmentStatementId == 0 ? LatestCommitmentStatement : CommitmentStatements.FirstOrDefault(x => x.Id == commitmentStatementId);

        internal void ConfirmEmployer(long commitmentStatementId, bool employerCorrect)
            => GetCommitmentStatement(commitmentStatementId).ConfirmEmployer(employerCorrect);

        internal void ConfirmApprenticeship(long commitmentStatementId, bool apprenticeshipCorrect)
            => GetCommitmentStatement(commitmentStatementId).ConfirmApprenticeship(apprenticeshipCorrect);

        internal void ConfirmApprenticeshipDetails(bool apprenticeshipCorrect)
            => LatestCommitmentStatement.ConfirmApprenticeshipDetails(apprenticeshipCorrect);

        internal void ConfirmApprenticeshipDetails(long commitmentStatementId, bool apprenticeshipCorrect)
            => GetCommitmentStatement(commitmentStatementId).ConfirmApprenticeshipDetails(apprenticeshipCorrect);

        internal void ConfirmTrainingProvider(long commitmentStatementId, bool apprenticeshipCorrect)
            => GetCommitmentStatement(commitmentStatementId).ConfirmTrainingProvider(apprenticeshipCorrect);

        internal void ConfirmRolesAndResponsibilities(long commitmentStatementId, bool apprenticeshipCorrect)
            => GetCommitmentStatement(commitmentStatementId).ConfirmRolesAndResponsibilities(apprenticeshipCorrect);

        internal void ConfirmHowApprenticeshipWillBeDelivered(long commitmentStatementId, bool apprenticeshipCorrect)
            => GetCommitmentStatement(commitmentStatementId).ConfirmHowApprenticeshipWillBeDelivered(apprenticeshipCorrect);

        internal void RenewCommitment(ApprenticeshipDetails details, DateTime approvedOn)
        {
            var ns = CommitmentStatements.OrderByDescending(x => x.CommitmentsApprovedOn).First().RenewCommitment(details, approvedOn);
            CommitmentStatements.Add(ns);
        }
    }
}