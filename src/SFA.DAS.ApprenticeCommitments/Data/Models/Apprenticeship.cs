using MediatR;
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

        public void RenewCommitment(long commitmentsApprenticeshipId, ApprenticeshipDetails details, DateTime approvedOn)
        {
            var renewed = LatestCommitmentStatement.Renew(commitmentsApprenticeshipId, approvedOn, details);

            if (renewed != null)
            {
                CommitmentStatements.Add(renewed);

                var e = new ApprenticeshipConfirmationCommenced
                {
                    ApprenticeshipId = Id,
                    ConfirmationOverdueOn = renewed.ConfirmBefore,
                    CommitmentsApprenticeshipId = commitmentsApprenticeshipId,
                    CommitmentsApprovedOn = approvedOn,
                };

                AddDomainEvent(e);
            }
        }
    }

    public abstract class Entity
    {
        public List<INotification> DomainEvents { get; }
            = new List<INotification>();

        public void AddDomainEvent(INotification eventItem)
        {
            DomainEvents.Add(eventItem);
        }

        public void RemoveDomainEvent(INotification eventItem)
        {
            DomainEvents.Remove(eventItem);
        }
    }
}