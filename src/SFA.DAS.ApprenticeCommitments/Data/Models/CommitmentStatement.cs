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
        public bool ApprenticeshipConfirmed { get; private set; }
        public DateTime ConfirmedOn { get; private set; }

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

        public void ConfirmApprenticeship(bool apprenticeshipCorrect)
        {
            ApprenticeshipConfirmed = apprenticeshipCorrect;
            ConfirmedOn = DateTime.Now.Date;
        }

        public CommitmentStatement RenewCommitment(long commitmentsApprenticeshipId, ApprenticeshipDetails updatedDetails, DateTime approvedOn)
        {
            return new CommitmentStatement
            {
                ApprenticeshipId = ApprenticeshipId,
                CommitmentsApprenticeshipId = commitmentsApprenticeshipId,
                CommitmentsApprovedOn = approvedOn,
                Details = updatedDetails,
            };
        }
    }
}