﻿using SFA.DAS.ApprenticeCommitments.Exceptions;
using System;
using System.ComponentModel.DataAnnotations.Schema;

#nullable enable

namespace SFA.DAS.ApprenticeCommitments.Data.Models
{
    [Table("CommitmentStatement")]
    public class CommitmentStatement
    {
        private CommitmentStatement()
        {
        }

        public CommitmentStatement(long commitmentsApprenticeshipId,
            DateTime approvedOn,
            ApprenticeshipDetails details)
        {
            CommitmentsApprenticeshipId = commitmentsApprenticeshipId;
            CommitmentsApprovedOn = approvedOn;
            Details = details;
        }

        public long Id { get; private set; }
        public long ApprenticeshipId { get; private set; }
        public ApprenticeshipDetails Details { get; private set; } = null!;
        public long CommitmentsApprenticeshipId { get; private set; }
        public DateTime CommitmentsApprovedOn { get; private set; }
        public Apprenticeship Apprenticeship { get; internal set; } = null!;

        public bool? TrainingProviderCorrect { get; private set; }
        public bool? EmployerCorrect { get; private set; }
        public bool? RolesAndResponsibilitiesCorrect { get; private set; }
        public bool? ApprenticeshipDetailsCorrect { get; private set; }
        public bool? HowApprenticeshipDeliveredCorrect { get; private set; }
        public bool ApprenticeshipConfirmed => ConfirmedOn.HasValue;
        public DateTime? ConfirmedOn { get; private set; }

        public void ConfirmTrainingProvider(bool trainingProviderCorrect)
        {
            TrainingProviderCorrect = trainingProviderCorrect;
        }

        public void ConfirmEmployer(bool employerCorrect)
        {
            EmployerCorrect = employerCorrect;
        }

        public void ConfirmRolesAndResponsibilities(bool rolesAndResponsibilitiesCorrect)
        {
            RolesAndResponsibilitiesCorrect = rolesAndResponsibilitiesCorrect;
        }

        public void ConfirmApprenticeshipDetails(bool apprenticeshipDetailsCorrect)
        {
            ApprenticeshipDetailsCorrect = apprenticeshipDetailsCorrect;
        }

        public void ConfirmHowApprenticeshipWillBeDelivered(bool howApprenticeshipDeliveredCorrect)
        {
            HowApprenticeshipDeliveredCorrect = howApprenticeshipDeliveredCorrect;
        }

        public void ConfirmApprenticeship(bool apprenticeshipCorrect, DateTimeOffset time)
        {
            if (apprenticeshipCorrect
                && TrainingProviderCorrect == true
                && EmployerCorrect == true
                && RolesAndResponsibilitiesCorrect == true
                && ApprenticeshipDetailsCorrect == true
                && HowApprenticeshipDeliveredCorrect == true)
            {
                ConfirmedOn = time.UtcDateTime;
            }
            else
            {
                throw new DomainException($"Cannot confirm apprenticeship `{ApprenticeshipId}` ({Id}) with unconfirmed section(s).");
            }
        }

        public void RenewedFromCommitment(CommitmentStatement lastStatement)
        {

            bool EmployerIsEquivalent()
            {
                return lastStatement.Details.EmployerAccountLegalEntityId ==
                       Details.EmployerAccountLegalEntityId
                       && lastStatement.Details.EmployerName == Details.EmployerName;
            }

            bool ProviderIsEquivalent()
            {
                return lastStatement.Details.TrainingProviderId == Details.TrainingProviderId &&
                        lastStatement.Details.TrainingProviderName == Details.TrainingProviderName;
            }

            bool ApprenticeshipIsEquivalent()
            {
                return lastStatement.CommitmentsApprenticeshipId == CommitmentsApprenticeshipId 
                       && Details.Course.IsEquivalent(lastStatement.Details.Course);
            }

            if (lastStatement == null) throw new ArgumentNullException(nameof(lastStatement));

            if (lastStatement.EmployerCorrect.HasValue && EmployerIsEquivalent())
            {
                EmployerCorrect = lastStatement.EmployerCorrect;
            }

            if (lastStatement.TrainingProviderCorrect.HasValue && ProviderIsEquivalent())
            {
                TrainingProviderCorrect = lastStatement.TrainingProviderCorrect;
            }

            if (lastStatement.ApprenticeshipDetailsCorrect.HasValue && ApprenticeshipIsEquivalent())
            {
                ApprenticeshipDetailsCorrect = lastStatement.ApprenticeshipDetailsCorrect;
            }

            if (lastStatement.HowApprenticeshipDeliveredCorrect.HasValue)
            {
                HowApprenticeshipDeliveredCorrect = lastStatement.HowApprenticeshipDeliveredCorrect;
            }

            if (lastStatement.RolesAndResponsibilitiesCorrect.HasValue)
            {
                RolesAndResponsibilitiesCorrect = lastStatement.RolesAndResponsibilitiesCorrect;
            }
        }
    }
}