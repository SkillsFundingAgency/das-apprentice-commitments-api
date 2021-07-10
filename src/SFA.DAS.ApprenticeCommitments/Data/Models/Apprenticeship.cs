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

                var statements = CommitmentStatements.OrderBy(x => x.CommitmentsApprovedOn).ToList();
                var latest = statements[CommitmentStatements.Count - 1];
                var previous = statements[CommitmentStatements.Count - 2];

                if (latest.EmployerCorrect == null &&
                    !latest.Details.EmployerIsEquivalent(previous.Details))
                {
                    return true;
                }

                if (latest.TrainingProviderCorrect == null &&
                    !latest.Details.ProviderIsEquivalent(previous.Details))
                {
                    return true;
                }

                if (latest.ApprenticeshipDetailsCorrect == null &&
                    !latest.Details.ApprenticeshipIsEquivalent(previous.Details))
                {
                    return true;
                }

                return false;
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