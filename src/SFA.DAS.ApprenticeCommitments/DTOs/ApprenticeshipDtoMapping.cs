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
                EmployerCorrect = latest.EmployerCorrect,
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

        [return: NotNullIfNotNull("apprenticeship")]
        public static ApprenticeshipDto? MapToApprenticeshipDto(this CommitmentStatement? apprenticeship)
        {
            if (apprenticeship == null) return null;

            return new ApprenticeshipDto
            {
                Id = apprenticeship.ApprenticeshipId,
                ApprenticeId = apprenticeship.Apprenticeship.ApprenticeId,
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
                ConfirmedOn = apprenticeship.ConfirmedOn,
                ConfirmBefore = apprenticeship.ConfirmBefore,
                DisplayChangeNotification = apprenticeship.DisplayChangeNotification,
                ApprovedOn = apprenticeship.CommitmentsApprovedOn,
            };
        }
    }
}