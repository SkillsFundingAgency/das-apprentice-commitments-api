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

        public bool? TrainingProviderCorrect { get; private set; }
        public bool? RolesAndResponsibilitiesCorrect { get; private set; }
        public bool? ApprenticeshipDetailsCorrect { get; private set; }
        public bool? HowApprenticeshipDeliveredCorrect { get; private set; }
        public bool ApprenticeshipConfirmed => ConfirmedOn.HasValue;

        public bool DisplayChangeNotification => !(EmployerCorrect?.ConfirmedOn < CommitmentsApprovedOn);

        public DateTime ConfirmBefore { get; private set; }
        public DateTime? ConfirmedOn { get; private set; }

        public void Confirm(Confirmations confirmations, DateTimeOffset time)
        {
            EmployerCorrect = (Confirmation?)confirmations.EmployerCorrect ?? EmployerCorrect;
            TrainingProviderCorrect = confirmations.TrainingProviderCorrect ?? TrainingProviderCorrect;
            ApprenticeshipDetailsCorrect = confirmations.ApprenticeshipDetailsCorrect ?? ApprenticeshipDetailsCorrect;
            RolesAndResponsibilitiesCorrect = confirmations.RolesAndResponsibilitiesCorrect ?? RolesAndResponsibilitiesCorrect;
            HowApprenticeshipDeliveredCorrect = confirmations.HowApprenticeshipDeliveredCorrect ?? HowApprenticeshipDeliveredCorrect;

            if (confirmations.ApprenticeshipCorrect == true)
            {
                if (TrainingProviderCorrect == true
                    && EmployerCorrect
                    && RolesAndResponsibilitiesCorrect == true
                    && ApprenticeshipDetailsCorrect == true
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

    public sealed class Confirmation
    {
        private Confirmation()
        {
        }

        private Confirmation(bool correct) =>
            (Correct, ConfirmedOn) = (correct, correct ? default : DateTime.UtcNow);

        public Confirmation Clone() =>
            new Confirmation { Correct = Correct, ConfirmedOn = ConfirmedOn };

        public bool? Correct { get; private set; }

        public DateTime? ConfirmedOn { get; private set; }

        public static implicit operator Confirmation?(bool? correct) =>
            correct == null ? default : new Confirmation(correct.Value);

        public static implicit operator bool(Confirmation? confirmation) =>
            confirmation?.Correct ?? false;
    }
}