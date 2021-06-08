using SFA.DAS.ApprenticeCommitments.Data.Models;
using System.Diagnostics.CodeAnalysis;

#nullable enable

namespace SFA.DAS.ApprenticeCommitments.DTOs
{
    public static class ApprenticeshipDtoMapping
    {
        [return: NotNullIfNotNull("apprenticeship")]
        public static ApprenticeshipDto? MapToApprenticeshipDto(this CommitmentStatement? apprenticeship)
        {
            if (apprenticeship == null) return null;

            return new ApprenticeshipDto
            {
                Id = apprenticeship.ApprenticeshipId,
                CommitmentStatementId = apprenticeship.Id,
                CommitmentsApprenticeshipId = apprenticeship.CommitmentsApprenticeshipId,
                EmployerName = apprenticeship.Details.EmployerName,
                EmployerAccountLegalEntityId = apprenticeship.Details.EmployerAccountLegalEntityId,
                TrainingProviderId = apprenticeship.Details.TrainingProviderId,
                TrainingProviderName = apprenticeship.Details.TrainingProviderName,
                TrainingProviderCorrect = apprenticeship.TrainingProviderCorrect,
                ApprenticeshipDetailsCorrect = apprenticeship.ApprenticeshipDetailsCorrect,
                HowApprenticeshipDeliveredCorrect = apprenticeship.HowApprenticeshipDeliveredCorrect,
                EmployerCorrect = apprenticeship.EmployerCorrect,
                RolesAndResponsibilitiesCorrect = apprenticeship.RolesAndResponsibilitiesCorrect,
                CourseName = apprenticeship.Details.Course.Name,
                CourseLevel = apprenticeship.Details.Course.Level,
                CourseOption = apprenticeship.Details.Course.Option,
                PlannedStartDate = apprenticeship.Details.Course.PlannedStartDate,
                PlannedEndDate = apprenticeship.Details.Course.PlannedEndDate,
                DurationInMonths = apprenticeship.Details.Course.DurationInMonths,
                ConfirmedOn = apprenticeship.ConfirmedOn
            };
        }
    }
}