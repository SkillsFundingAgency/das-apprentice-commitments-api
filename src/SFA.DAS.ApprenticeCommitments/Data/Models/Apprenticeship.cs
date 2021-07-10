using SFA.DAS.ApprenticeCommitments.Application.DomainEvents;
using SFA.DAS.ApprenticeCommitments.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable

namespace SFA.DAS.ApprenticeCommitments.Data.Models
{
    public class Apprenticeship : Entity
    {
        private Apprenticeship()
        {
        }

        public Apprenticeship(CommitmentStatement apprenticeship)
        {
            AddCommitmentStatement(apprenticeship);
        }

        public long Id { get; private set; }

        public Guid ApprenticeId { get; private set; }

        public DateTime LastViewed { get; set; }

        private readonly List<CommitmentStatement> _commitmentStatements = new List<CommitmentStatement>();

        public IReadOnlyCollection<CommitmentStatement> CommitmentStatements => _commitmentStatements;

        public CommitmentStatement LatestCommitmentStatement
            => CommitmentStatements.OrderByDescending(x => x.CommitmentsApprovedOn).FirstOrDefault()
                ?? throw new DomainException($"No commitment statements found in apprenticeship {Id}");

        public bool DisplayChangeNotification
        {
            get
            {
                if (CommitmentStatements.Count == 1) return false;

                var previous = CommitmentStatements.ElementAt(CommitmentStatements.Count - 2);

                if (LatestCommitmentStatement.EmployerCorrect == null && !EmployerIsEquivalent(LatestCommitmentStatement, previous)) return true;
                if (LatestCommitmentStatement.TrainingProviderCorrect == null && !ProviderIsEquivalent(LatestCommitmentStatement, previous)) return true;
                if (LatestCommitmentStatement.ApprenticeshipDetailsCorrect == null && !ApprenticeshipIsEquivalent(LatestCommitmentStatement, previous)) return true;

                return false;

                bool EmployerIsEquivalent(CommitmentStatement l, CommitmentStatement r) =>
                    l.Details.EmployerAccountLegalEntityId == r.Details.EmployerAccountLegalEntityId &&
                    l.Details.EmployerName == r.Details.EmployerName;

                bool ProviderIsEquivalent(CommitmentStatement l, CommitmentStatement r) =>
                    l.Details.TrainingProviderId == r.Details.TrainingProviderId &&
                    l.Details.TrainingProviderName == r.Details.TrainingProviderName;

                bool ApprenticeshipIsEquivalent(CommitmentStatement l, CommitmentStatement r) =>
                    l.Details.Course.IsEquivalent(r.Details.Course);
            }
        }

        private void AddCommitmentStatement(CommitmentStatement apprenticeship)
            => _commitmentStatements.Add(apprenticeship);

        private CommitmentStatement GetCommitmentStatement(long commitmentStatementId)
        {
            // Remove around the end of May 2021
            // https://skillsfundingagency.atlassian.net/browse/CS-655
            if (commitmentStatementId == 0)
            {
                return LatestCommitmentStatement;
            }
            else
            {
                return CommitmentStatements.FirstOrDefault(x => x.Id == commitmentStatementId)
                    ?? throw new DomainException(
                        $"Commitment Statement {commitmentStatementId} not found in apprenticeship {Id}");
            }
        }

        internal void Confirm(long commitmentStatementId, Confirmations confirmations, DateTimeOffset now)
            => GetCommitmentStatement(commitmentStatementId).Confirm(confirmations, now);

        public void RenewCommitment(long commitmentsApprenticeshipId, ApprenticeshipDetails details, DateTime approvedOn)
        {
            var renewed = LatestCommitmentStatement.Renew(commitmentsApprenticeshipId, approvedOn, details);
            if (renewed != null)
            {
                AddCommitmentStatement(renewed);
                AddDomainEvent(new ApprenticeshipChanged(this));
            }
        }
    }
}