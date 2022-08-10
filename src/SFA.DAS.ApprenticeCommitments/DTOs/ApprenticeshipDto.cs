using SFA.DAS.ApprenticeCommitments.Data.Models;
using System;
using System.Collections.Generic;

#nullable disable

namespace SFA.DAS.ApprenticeCommitments.DTOs
{
    public class ApprenticeshipDto
    {
        public long Id { get; set; }
        public Guid ApprenticeId { get; set; }
        public long CommitmentsApprenticeshipId { get; set; }
        public string EmployerName { get; set; }
        public long EmployerAccountLegalEntityId { get; set; }
        public long TrainingProviderId { get; set; }
        public string TrainingProviderName { get; set; }
        public bool? TrainingProviderCorrect { get; set; }
        public bool? EmployerCorrect { get; set; }
        public RolesAndResponsibilitiesConfirmations RolesAndResponsibilitiesConfirmations { get; set; }
        public bool? ApprenticeshipDetailsCorrect { get; set; }
        public bool? HowApprenticeshipDeliveredCorrect { get; set; }
        public DeliveryModel DeliveryModel { get; set; }
        public string CourseName { get; set; }
        public int CourseLevel { get; set; }
        public string CourseOption { get; set; }
        public int CourseDuration { get; set; }
        public DateTime PlannedStartDate { get; set; }
        public DateTime PlannedEndDate { get; set; }
        public DateTime? EmploymentEndDate { get; set; }
        public DateTime ConfirmBefore { get; set; }
        public DateTime? ConfirmedOn { get; set; }
        public long RevisionId { get; set; }
        public ChangeOfCircumstanceNotifications ChangeOfCircumstanceNotifications { get; set; }
        public DateTime ApprovedOn { get; set; }
        public DateTime? LastViewed { get; set; }
        public DateTime? StoppedReceivedOn { get; set; }
        public bool IsStopped => StoppedReceivedOn != null;
        public bool HasBeenConfirmedAtLeastOnce { get; set; }
        public List<TimelineDto> Timelines { get; set; }
    }
}