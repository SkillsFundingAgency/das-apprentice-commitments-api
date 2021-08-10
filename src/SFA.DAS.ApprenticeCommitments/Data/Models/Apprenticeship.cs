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

        public Apprenticeship(Revision apprenticeship)
        {
            AddRevision(apprenticeship);
        }

        public long Id { get; private set; }

        public Guid ApprenticeId { get; private set; }

        public DateTime LastViewed { get; set; }

        private readonly List<Revision> _revisions = new List<Revision>();

        public IReadOnlyCollection<Revision> Revisions => _revisions;

        public Revision LatestRevision
            => Revisions.OrderByDescending(x => x.CommitmentsApprovedOn).FirstOrDefault()
                ?? throw new DomainException($"No revisions found in apprenticeship {Id}");

        public bool DisplayChangeNotification
        {
            get
            {
                if (Revisions.Count == 1) return false;

                var revisions = Revisions.OrderBy(x => x.CommitmentsApprovedOn).ToList();
                var latest = revisions[Revisions.Count - 1];
                var previous = revisions[Revisions.Count - 2];

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

        private void AddRevision(Revision apprenticeship)
            => _revisions.Add(apprenticeship);

        private Revision GetRevision(long revisionId)
        {
            // Remove around the end of May 2021
            // https://skillsfundingagency.atlassian.net/browse/CS-655
            if (revisionId == 0)
            {
                return LatestRevision;
            }
            else
            {
                return Revisions.FirstOrDefault(x => x.Id == revisionId)
                    ?? throw new DomainException(
                        $"Apprenticeship {Id} revision {revisionId} not found");
            }
        }

        internal void Confirm(long revisionId, Confirmations confirmations, DateTimeOffset now)
            => GetRevision(revisionId).Confirm(confirmations, now);

        public void RenewCommitment(long commitmentsApprenticeshipId, ApprenticeshipDetails details, DateTime approvedOn)
        {
            var renewed = LatestRevision.Renew(commitmentsApprenticeshipId, approvedOn, details);
            if (renewed != null)
            {
                AddRevision(renewed);
                AddDomainEvent(new ApprenticeshipChanged(this));
            }
        }
    }
}