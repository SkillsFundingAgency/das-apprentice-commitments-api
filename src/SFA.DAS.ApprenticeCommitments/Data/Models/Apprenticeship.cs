using SFA.DAS.ApprenticeCommitments.Exceptions;
using SFA.DAS.ApprenticeCommitments.Messages.Events;
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
            => CommitmentStatements.OrderByDescending(x => x.CommitmentsApprovedOn).FirstOrDefault()
                ?? throw new DomainException($"No commitment statements found in apprenticeship {Id}");

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

        public object? RenewCommitment(long commitmentsApprenticeshipId, ApprenticeshipDetails details, DateTime approvedOn)
        {
            var renewed = LatestCommitmentStatement.Renew(commitmentsApprenticeshipId, approvedOn, details);

            if (renewed == null) return null;

            CommitmentStatements.Add(renewed);

            return new ApprenticeshipConfirmationCommencedEvent
            {
                ApprenticeshipId = Id,
                ConfirmationOverdueOn = renewed.ConfirmBefore,
                CommitmentsApprenticeshipId = commitmentsApprenticeshipId,
                CommitmentsApprovedOn = approvedOn,
            };
        }
    }
}