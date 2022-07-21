using MoreLinq;
using SFA.DAS.ApprenticeCommitments.Data.Models;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

#nullable enable

namespace SFA.DAS.ApprenticeCommitments.DTOs
{
    public static class ApprenticeshipDtoMapping
    {
        [return: NotNullIfNotNull("apprenticeship")]
        public static ApprenticeshipDto? MapToApprenticeshipDto(this Apprenticeship? apprenticeship)
        {
            if (apprenticeship == null) return null;

            var latest = apprenticeship.LatestRevision;
            return MapApprenticeshipAndRevisionToApprenticeshipDto(apprenticeship, latest);
        }

        public static ApprenticeshipDto? MapToMyApprenticeshipDto(this Apprenticeship? apprenticeship)
        {
            if (apprenticeship == null) return null;

            var latest = apprenticeship.LatestConfirmedRevision;
            if (latest == null)
                return null;
            return MapApprenticeshipAndRevisionToApprenticeshipDto(apprenticeship, latest, true);
        }

        private static ApprenticeshipDto MapApprenticeshipAndRevisionToApprenticeshipDto(Apprenticeship apprenticeship, Revision latest, bool includeTimeline = false)
        {
            return new ApprenticeshipDto
            {
                Id = apprenticeship.Id,
                ApprenticeId = apprenticeship.ApprenticeId,
                RevisionId = latest.Id,
                LastViewed = latest.LastViewed,
                CommitmentsApprenticeshipId = latest.CommitmentsApprenticeshipId,
                EmployerName = latest.Details.EmployerName,
                EmployerAccountLegalEntityId = latest.Details.EmployerAccountLegalEntityId,
                TrainingProviderId = latest.Details.TrainingProviderId,
                TrainingProviderName = latest.Details.TrainingProviderName,
                TrainingProviderCorrect = latest.TrainingProviderCorrect,
                ApprenticeshipDetailsCorrect = latest.ApprenticeshipDetailsCorrect,
                HowApprenticeshipDeliveredCorrect = latest.HowApprenticeshipDeliveredCorrect,
                EmployerCorrect = latest.EmployerCorrect,
                RolesAndResponsibilitiesConfirmations = latest.RolesAndResponsibilitiesConfirmations ?? RolesAndResponsibilitiesConfirmations.None,
                DeliveryModel = latest.Details.DeliveryModel,
                CourseName = latest.Details.Course.Name,
                CourseLevel = latest.Details.Course.Level,
                CourseOption = latest.Details.Course.Option,
                CourseDuration = latest.Details.Course.CourseDuration,
                PlannedStartDate = latest.Details.Course.PlannedStartDate,
                PlannedEndDate = latest.Details.Course.PlannedEndDate,
                EmploymentEndDate = latest.Details.Course.EmploymentEndDate,
                ConfirmedOn = latest.ConfirmedOn,
                ConfirmBefore = latest.ConfirmBefore,
                ApprovedOn = latest.CommitmentsApprovedOn,
                ChangeOfCircumstanceNotifications = apprenticeship.ChangeOfCircumstanceNotifications,
                StoppedReceivedOn = latest.StoppedReceivedOn,
                HasBeenConfirmedAtLeastOnce = apprenticeship.ApprenticeshipHasPreviouslyBeenConfirmed,
                Timelines = includeTimeline ? GetTimeline(apprenticeship.Revisions, latest) : null,
            };
        }

        private static List<TimelineDto> GetTimeline(IReadOnlyCollection<Revision> revisions, Revision latest)
        { 
            var ret = new List<TimelineDto>();
            if (revisions.Count == 1)
                return ret;

            var confirmedRevisions = revisions.Where(x => x.ConfirmedOn.HasValue).ToList();
            if (confirmedRevisions.Count() <= 1)
                return ret;

            var rev = confirmedRevisions.OrderByDescending(x => x.Id).Pairwise((newer, older) =>
            {
                if (newer.Details.DeliveryModel != older.Details.DeliveryModel)
                    ret.Add(new TimelineDto("Delivery model changed",                                                
                         $"Delivery model changed from {older.Details.DeliveryModel} to {newer.Details.DeliveryModel}",
                         newer.CreatedOn));

                if (newer.Details.EmployerName != older.Details.EmployerName)
                    ret.Add(new TimelineDto("You started with a new employer",
                        $"Employer changed from {older.Details.EmployerName} to {newer.Details.EmployerName}",
                        newer.CreatedOn));

                if (newer.Details.TrainingProviderName != older.Details.TrainingProviderName)
                    ret.Add(new TimelineDto("You started with a new training provider",
                        $"Provider changed from {older.Details.TrainingProviderName} to {newer.Details.TrainingProviderName}",
                        newer.CreatedOn));

                return newer;
            }).ToList();

            var confirmedOn = confirmedRevisions.OrderBy(x => x.Id).FirstOrDefault().ConfirmedOn;

            if (ret.Count > 0)
                ret.Add(new TimelineDto("You started your apprenticeship",
                    "You confirmed your apprenticeship details",
                    confirmedOn));

            return ret;
        }
    }
}
