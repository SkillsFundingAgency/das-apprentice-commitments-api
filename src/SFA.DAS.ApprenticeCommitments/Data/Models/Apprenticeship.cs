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

        internal void Confirm(long commitmentStatementId, Confirmations confirmations)
            => GetCommitmentStatement(commitmentStatementId).Confirm(confirmations);

        internal void RenewCommitment(ApprenticeshipDetails details, DateTime approvedOn)
        {
            var ns = CommitmentStatements.OrderByDescending(x => x.CommitmentsApprovedOn).First().RenewCommitment(details, approvedOn);
            CommitmentStatements.Add(ns);
        }
    }
}