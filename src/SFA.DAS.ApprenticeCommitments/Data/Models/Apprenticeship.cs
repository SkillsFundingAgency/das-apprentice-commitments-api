using System;
using System.ComponentModel.DataAnnotations.Schema;

#nullable enable

namespace SFA.DAS.ApprenticeCommitments.Data.Models
{
    [Table("Apprenticeship")]
    public class Apprenticeship
    {
#pragma warning disable CS8618 // Constructor for Entity Framework

        private Apprenticeship()
#pragma warning restore CS8618
        {
        }

        public Apprenticeship(long commitmentsApprenticeshipId,
            DateTime approvedOn,
            ApprenticeshipDetails details)
        {
            CommitmentsApprenticeshipId = commitmentsApprenticeshipId;
            ApprovedOn = approvedOn;
            Details = details;
        }

        public long ApprenticeshipId { get; private set; } = 0;
        public long CommitmentsApprenticeshipId { get; private set; }
        public Apprentice Apprentice { get; private set; }
        public ApprenticeshipDetails Details { get; private set; }

        public bool? TrainingProviderCorrect { get; private set; }
        public bool? EmployerCorrect { get; private set; }
        public bool? RolesAndResponsibilitiesCorrect { get; private set; }
        public bool? ApprenticeshipDetailsCorrect { get; private set; }
        public bool? HowApprenticeshipDeliveredCorrect { get; private set; }
        public bool? ApprenticeshipConfirmed { get; private set; }
        public DateTime ApprovedOn { get; private set; }

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
        }

        public Apprenticeship RenewCommitment(ApprenticeshipDetails updatedDetails, DateTime approvedOn)
        {
            return new Apprenticeship
            {
                ApprenticeshipId = ApprenticeshipId,
                CommitmentsApprenticeshipId = CommitmentsApprenticeshipId,
                ApprovedOn = approvedOn,
                Details = updatedDetails,
            };
        }
    }
}