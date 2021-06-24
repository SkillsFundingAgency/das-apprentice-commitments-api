using SFA.DAS.ApprenticeCommitments.Data.Models;
using System.Diagnostics.CodeAnalysis;

#nullable enable

namespace SFA.DAS.ApprenticeCommitments.DTOs
{
    public static class ApprenticeshipDtoMapping
    {
        [return: NotNullIfNotNull("apprenticeship")]
        public static ApprenticeshipDto? MapToApprenticeshipDto(this Apprenticeship? apprenticeship)
        {
            if (apprenticeship == null) return null;

            var latest = apprenticeship.LatestCommitmentStatement;

            return new ApprenticeshipDto
            {
                Id = apprenticeship.Id,
                ApprenticeId = apprenticeship.ApprenticeId,
                CommitmentStatementId = latest.Id,
                CommitmentsApprenticeshipId = latest.CommitmentsApprenticeshipId,
                EmployerName = latest.Details.EmployerName,
                EmployerAccountLegalEntityId = latest.Details.EmployerAccountLegalEntityId,
                TrainingProviderId = latest.Details.TrainingProviderId,
                TrainingProviderName = latest.Details.TrainingProviderName,
                TrainingProviderCorrect = latest.TrainingProviderCorrect,
                ApprenticeshipDetailsCorrect = latest.ApprenticeshipDetailsCorrect,
                HowApprenticeshipDeliveredCorrect = latest.HowApprenticeshipDeliveredCorrect,
                EmployerCorrect = latest.EmployerCorrect?.Correct,
                RolesAndResponsibilitiesCorrect = latest.RolesAndResponsibilitiesCorrect,
                CourseName = latest.Details.Course.Name,
                CourseLevel = latest.Details.Course.Level,
                CourseOption = latest.Details.Course.Option,
                PlannedStartDate = latest.Details.Course.PlannedStartDate,
                PlannedEndDate = latest.Details.Course.PlannedEndDate,
                DurationInMonths = latest.Details.Course.DurationInMonths,
                ConfirmedOn = latest.ConfirmedOn,
                ConfirmBefore = latest.ConfirmBefore,
                ApprovedOn = latest.CommitmentsApprovedOn,
                DisplayChangeNotification = apprenticeship.DisplayChangeNotification,
            };
        }
    }
}