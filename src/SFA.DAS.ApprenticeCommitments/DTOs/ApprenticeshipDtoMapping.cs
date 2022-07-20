﻿using MoreLinq;
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
            return MapApprenticeshipAndRevisionToApprenticeshipDto(apprenticeship, latest);
        }

        private static ApprenticeshipDto MapApprenticeshipAndRevisionToApprenticeshipDto(Apprenticeship apprenticeship, Revision latest)
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
                HasBeenConfirmedAtLeastOnce = apprenticeship.ApprenticeshipHasPreviouslyBeenConfirmed
                Revisions = GetRevisions(apprenticeship.Revisions),
            };
        }

        private static List<RevisionDto> GetRevisions(IReadOnlyCollection<Revision> revisions)
        { 
            var ret = new List<RevisionDto>();
            if (revisions.Count == 1)
                return ret;

            var rev = revisions.OrderByDescending(x => x.Id).Pairwise((x, y) =>
            {
                if (x.Details.DeliveryModel != y.Details.DeliveryModel)
                    ret.Add(new RevisionDto("Delivery model changed",                                                
                         $"Delivery model changed from {y.Details.DeliveryModel} to {x.Details.DeliveryModel}",
                         x.CreatedOn));

                if (x.Details.EmployerName != y.Details.EmployerName)
                    ret.Add(new RevisionDto("You started with a new employer",
                        $"Employer changed from {y.Details.EmployerName} to {x.Details.EmployerName}",
                        x.CreatedOn));

                if (x.Details.TrainingProviderName != y.Details.TrainingProviderName)
                    ret.Add(new RevisionDto("You started with a new training provider",
                        $"Provider changed from {y.Details.TrainingProviderName} to {x.Details.TrainingProviderName}",
                        x.CreatedOn));

                return x;
            }).ToList();

            var confirmedOn = revisions.FirstOrDefault(x => x.ConfirmedOn.HasValue)?.ConfirmedOn;

            if (confirmedOn.HasValue && ret.Count > 0)
                ret.Add(new RevisionDto("You started your apprenticeship",
                    "You confirmed your apprenticeship details",
                    confirmedOn));

            return ret;
        }
    }
}
