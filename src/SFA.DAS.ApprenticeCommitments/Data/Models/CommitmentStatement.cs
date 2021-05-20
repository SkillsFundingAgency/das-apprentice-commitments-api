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
        public long ApprenticeshipId { get; private set; } = 0;
        public ApprenticeshipDetails Details { get; private set; } = null!;
        public long CommitmentsApprenticeshipId { get; private set; }
        public DateTime CommitmentsApprovedOn { get; private set; }
        public Apprenticeship Apprenticeship { get; internal set; } = null!;

        public bool? TrainingProviderCorrect { get; private set; }
        public bool? EmployerCorrect { get; private set; }
        public bool? RolesAndResponsibilitiesCorrect { get; private set; }
        public bool? ApprenticeshipDetailsCorrect { get; private set; }
        public bool? HowApprenticeshipDeliveredCorrect { get; private set; }
        public bool? ApprenticeshipConfirmed { get; private set; }

        public void ConfirmTrainingProvider(bool trainingProviderCorrect)
        {
            TrainingProviderCorrect = trainingProviderCorrect;
        }

        public void Confirm(Confirmations confirmations)
        {
            EmployerCorrect = confirmations.EmployerCorrect ?? EmployerCorrect;
            TrainingProviderCorrect = confirmations.TrainingProviderCorrect ?? TrainingProviderCorrect;
            ApprenticeshipDetailsCorrect = confirmations.ApprenticeshipDetailsCorrect ?? ApprenticeshipDetailsCorrect;
            RolesAndResponsibilitiesCorrect = confirmations.RolesAndResponsibilitiesCorrect ?? RolesAndResponsibilitiesCorrect;
            HowApprenticeshipDeliveredCorrect = confirmations.HowApprenticeshipDeliveredCorrect ?? HowApprenticeshipDeliveredCorrect;
            ApprenticeshipConfirmed = confirmations.ApprenticeshipCorrect ?? ApprenticeshipConfirmed;
        }

        public CommitmentStatement RenewCommitment(ApprenticeshipDetails updatedDetails, DateTime approvedOn)
        {
            return new CommitmentStatement
            {
                ApprenticeshipId = ApprenticeshipId,
                CommitmentsApprenticeshipId = CommitmentsApprenticeshipId,
                CommitmentsApprovedOn = approvedOn,
                Details = updatedDetails,
            };
        }
    }
}