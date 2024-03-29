﻿using SFA.DAS.ApprenticeCommitments.Application.DomainEvents;
using SFA.DAS.ApprenticeCommitments.Exceptions;
using System;

#nullable enable

namespace SFA.DAS.ApprenticeCommitments.Data.Models
{
    public class Revision : Entity, IStoppable
    {
        public static int DaysBeforeOverdue { get; set; } = 14;

        private Revision()
        {
        }

        public Revision(long commitmentsApprenticeshipId,
            DateTime approvedOn,
            ApprenticeshipDetails details,
            DateTime? stoppedReceivedOn = null)
        {
            CommitmentsApprenticeshipId = commitmentsApprenticeshipId;
            CommitmentsApprovedOn = approvedOn;
            ConfirmBefore = CommitmentsApprovedOn.AddDays(DaysBeforeOverdue);
            Details = details;
            StoppedReceivedOn = stoppedReceivedOn;

            AddDomainEvent(new RevisionAdded(this));
        }

        public long Id { get; private set; }
        public long ApprenticeshipId { get; private set; }
        public ApprenticeshipDetails Details { get; private set; } = null!;
        public long CommitmentsApprenticeshipId { get; private set; }
        public DateTime CommitmentsApprovedOn { get; private set; }
        public Apprenticeship Apprenticeship { get; internal set; } = null!;

        public bool? EmployerCorrect { get; private set; }
        public bool? TrainingProviderCorrect { get; private set; }
        public bool? ApprenticeshipDetailsCorrect { get; private set; }

        public RolesAndResponsibilitiesConfirmations? RolesAndResponsibilitiesConfirmations { get; private set; }
        public bool? HowApprenticeshipDeliveredCorrect { get; private set; }
        public bool ApprenticeshipConfirmed => ConfirmedOn.HasValue;

        public DateTime ConfirmBefore { get; private set; }
        public DateTime? ConfirmedOn { get; private set; }
        public DateTime? LastViewed { get; set; }
        public DateTime? StoppedReceivedOn { get; private set; }

        public void Confirm(Confirmations confirmations, DateTimeOffset time)
        {
            EmployerCorrect = confirmations.EmployerCorrect ?? EmployerCorrect;
            TrainingProviderCorrect = confirmations.TrainingProviderCorrect ?? TrainingProviderCorrect;
            ApprenticeshipDetailsCorrect = confirmations.ApprenticeshipDetailsCorrect ?? ApprenticeshipDetailsCorrect;
            RolesAndResponsibilitiesConfirmations = DetermineRolesAndResponsibilitiesConfirmationsStatus(confirmations.RolesAndResponsibilitiesConfirmations);
            HowApprenticeshipDeliveredCorrect = confirmations.HowApprenticeshipDeliveredCorrect ?? HowApprenticeshipDeliveredCorrect;

            if (confirmations.ApprenticeshipCorrect == true)
            {
                if (EmployerCorrect == true
                    && TrainingProviderCorrect == true
                    && ApprenticeshipDetailsCorrect == true
                    && RolesAndResponsibilitiesConfirmations.IsConfirmed()
                    && HowApprenticeshipDeliveredCorrect == true)
                {
                    ConfirmRevision(time);
                }
                else
                {
                    throw new DomainException($"Cannot confirm apprenticeship `{ApprenticeshipId}` ({Id}) with unconfirmed section(s).");
                }
            }
        }

        private RolesAndResponsibilitiesConfirmations? DetermineRolesAndResponsibilitiesConfirmationsStatus(RolesAndResponsibilitiesConfirmations? rolesAndResponsibilitiesConfirmations)
        {
            if (rolesAndResponsibilitiesConfirmations == null)
                return RolesAndResponsibilitiesConfirmations;

            if (RolesAndResponsibilitiesConfirmations == null)
                return rolesAndResponsibilitiesConfirmations;

            return RolesAndResponsibilitiesConfirmations.Value | rolesAndResponsibilitiesConfirmations;
        }

        private void ConfirmRevision(DateTimeOffset time)
        {
            ConfirmedOn = time.UtcDateTime;
            AddDomainEvent(new RevisionConfirmed(this));
        }

        internal Revision? Renew(long commitmentsApprenticeshipId, DateTime approvedOn, ApprenticeshipDetails details)
        {
            bool EmployerIsEquivalent() => details.EmployerIsEquivalent(Details);
            bool ProviderIsEquivalent() => details.ProviderIsEquivalent(Details);
            bool ApprenticeshipIsEquivalent() => details.ApprenticeshipIsEquivalent(Details);
            bool DeliveryModelIsEquivalent() => details.DeliveryModelIsEquivalent(Details);

            if (Details.Equals(details)) return null;

            var revision = new Revision(commitmentsApprenticeshipId, approvedOn, details);

            if (EmployerIsEquivalent() && DeliveryModelIsEquivalent())  revision.EmployerCorrect = EmployerCorrect;
            if (ProviderIsEquivalent()) revision.TrainingProviderCorrect = TrainingProviderCorrect;
            if (ApprenticeshipIsEquivalent() && DeliveryModelIsEquivalent()) revision.ApprenticeshipDetailsCorrect = ApprenticeshipDetailsCorrect;
            if (DeliveryModelIsEquivalent()) revision.HowApprenticeshipDeliveredCorrect = HowApprenticeshipDeliveredCorrect;
            if (DeliveryModelIsEquivalent()) revision.RolesAndResponsibilitiesConfirmations = RolesAndResponsibilitiesConfirmations;

            if (!DeliveryModelIsEquivalent())
            {
                revision.EmployerCorrect = null;
                revision.ApprenticeshipDetailsCorrect = null;
                revision.HowApprenticeshipDeliveredCorrect = null;
                revision.RolesAndResponsibilitiesConfirmations = null;
                return revision;
            }

            return revision;
        }

        public void Stop(DateTime now)
        {
            StoppedReceivedOn = now;
            AddDomainEvent(new RevisionStopped(this));
        }
    }
}