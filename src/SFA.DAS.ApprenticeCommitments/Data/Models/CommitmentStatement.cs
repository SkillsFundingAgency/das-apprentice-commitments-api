using SFA.DAS.ApprenticeCommitments.Exceptions;
using System;
using System.ComponentModel.DataAnnotations.Schema;

#nullable enable

namespace SFA.DAS.ApprenticeCommitments.Data.Models
{
    [Table("CommitmentStatement")]
    public class CommitmentStatement
    {
        public static int DaysBeforeOverdue { get; set; } = 14;

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

        public DateTime ConfirmBefore => CommitmentsApprovedOn.AddDays(DaysBeforeOverdue);
        public DateTime? ConfirmedOn { get; private set; }

        public void Confirm(Confirmations confirmations, DateTimeOffset time)
        {
            EmployerCorrect = confirmations.EmployerCorrect ?? EmployerCorrect;
            TrainingProviderCorrect = confirmations.TrainingProviderCorrect ?? TrainingProviderCorrect;
            ApprenticeshipDetailsCorrect = confirmations.ApprenticeshipDetailsCorrect ?? ApprenticeshipDetailsCorrect;
            RolesAndResponsibilitiesCorrect = confirmations.RolesAndResponsibilitiesCorrect ?? RolesAndResponsibilitiesCorrect;
            HowApprenticeshipDeliveredCorrect = confirmations.HowApprenticeshipDeliveredCorrect ?? HowApprenticeshipDeliveredCorrect;

            if (confirmations.ApprenticeshipCorrect == true)
            {
                if (TrainingProviderCorrect == true
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
        }

        internal CommitmentStatement? Renew(long commitmentsApprenticeshipId, DateTime approvedOn, ApprenticeshipDetails details)
        {
            bool EmployerIsEquivalent() =>
                details.EmployerAccountLegalEntityId == Details.EmployerAccountLegalEntityId &&
                details.EmployerName == Details.EmployerName;

            bool ProviderIsEquivalent() =>
                details.TrainingProviderId == Details.TrainingProviderId &&
                details.TrainingProviderName == Details.TrainingProviderName;

            bool ApprenticeshipIsEquivalent() =>
                Details.Course.IsEquivalent(details.Course);

            if (EmployerIsEquivalent() && ProviderIsEquivalent() && ApprenticeshipIsEquivalent())
                return null;

            var newStatement = new CommitmentStatement(commitmentsApprenticeshipId, approvedOn, details);

            if (EmployerIsEquivalent()) newStatement.EmployerCorrect = EmployerCorrect;
            if (ProviderIsEquivalent()) newStatement.TrainingProviderCorrect = TrainingProviderCorrect;
            if (ApprenticeshipIsEquivalent()) newStatement.ApprenticeshipDetailsCorrect = ApprenticeshipDetailsCorrect;
            newStatement.HowApprenticeshipDeliveredCorrect = HowApprenticeshipDeliveredCorrect;
            newStatement.RolesAndResponsibilitiesCorrect = RolesAndResponsibilitiesCorrect;

            return newStatement;
        }
    }
}