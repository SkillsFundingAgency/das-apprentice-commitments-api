using SFA.DAS.ApprenticeCommitments.Application.DomainEvents;
using SFA.DAS.ApprenticeCommitments.Exceptions;
using System;
using System.ComponentModel.DataAnnotations.Schema;

#nullable enable

namespace SFA.DAS.ApprenticeCommitments.Data.Models
{
    [Table("CommitmentStatement")]
    public class CommitmentStatement : Entity
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
            ConfirmBefore = CommitmentsApprovedOn.AddDays(DaysBeforeOverdue);
            Details = details;

            AddDomainEvent(new CommitmentStatementAdded(this));
        }

        public long Id { get; private set; }
        public long ApprenticeshipId { get; private set; }
        public ApprenticeshipDetails Details { get; private set; } = null!;
        public long CommitmentsApprenticeshipId { get; private set; }
        public DateTime CommitmentsApprovedOn { get; private set; }
        public Apprenticeship Apprenticeship { get; internal set; } = null!;

        public Confirmation? EmployerCorrect { get; private set; }
        public Confirmation? TrainingProviderCorrect { get; private set; }
        public Confirmation? ApprenticeshipDetailsCorrect { get; private set; }

        public bool? RolesAndResponsibilitiesCorrect { get; private set; }
        public bool? HowApprenticeshipDeliveredCorrect { get; private set; }
        public bool ApprenticeshipConfirmed => ConfirmedOn.HasValue;

        public bool DisplayChangeNotification => EmployerCorrect == null;

        public DateTime ConfirmBefore { get; private set; }
        public DateTime? ConfirmedOn { get; private set; }

        public void Confirm(Confirmations confirmations, DateTimeOffset time)
        {
            EmployerCorrect = (Confirmation?)confirmations.EmployerCorrect ?? EmployerCorrect;
            TrainingProviderCorrect = (Confirmation?)confirmations.TrainingProviderCorrect ?? TrainingProviderCorrect;
            ApprenticeshipDetailsCorrect = (Confirmation?)confirmations.ApprenticeshipDetailsCorrect ?? ApprenticeshipDetailsCorrect;
            RolesAndResponsibilitiesCorrect = confirmations.RolesAndResponsibilitiesCorrect ?? RolesAndResponsibilitiesCorrect;
            HowApprenticeshipDeliveredCorrect = confirmations.HowApprenticeshipDeliveredCorrect ?? HowApprenticeshipDeliveredCorrect;

            if (confirmations.ApprenticeshipCorrect == true)
            {
                if (TrainingProviderCorrect && EmployerCorrect && ApprenticeshipDetailsCorrect
                    && RolesAndResponsibilitiesCorrect == true
                    && HowApprenticeshipDeliveredCorrect == true)
                {
                    ConfirmCommitmentStatement(time);
                }
                else
                {
                    throw new DomainException($"Cannot confirm apprenticeship `{ApprenticeshipId}` ({Id}) with unconfirmed section(s).");
                }
            }
        }

        private void ConfirmCommitmentStatement(DateTimeOffset time)
        {
            ConfirmedOn = time.UtcDateTime;
            AddDomainEvent(new CommitmentStatementConfirmed(this));
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

            if (Details.Equals(details)) return null;

            var newStatement = new CommitmentStatement(commitmentsApprenticeshipId, approvedOn, details);

            if (EmployerIsEquivalent()) newStatement.EmployerCorrect = EmployerCorrect?.Clone();
            if (ProviderIsEquivalent()) newStatement.TrainingProviderCorrect = TrainingProviderCorrect;
            if (ApprenticeshipIsEquivalent()) newStatement.ApprenticeshipDetailsCorrect = ApprenticeshipDetailsCorrect;
            newStatement.HowApprenticeshipDeliveredCorrect = HowApprenticeshipDeliveredCorrect;
            newStatement.RolesAndResponsibilitiesCorrect = RolesAndResponsibilitiesCorrect;

            return newStatement;
        }
    }
}