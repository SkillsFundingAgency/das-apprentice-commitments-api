#nullable enable

using System;

namespace SFA.DAS.ApprenticeCommitments.Data.Models
{
    public sealed class ApprenticeshipDetails : IEquatable<ApprenticeshipDetails>
    {
        private ApprenticeshipDetails()
        {
            // Private constructor for entity framework
        }

        public ApprenticeshipDetails(
            long employerAccountLegalEntityId, string employerName,
            long trainingProviderId, string trainingProviderName,
            DeliveryModel deliveryModel,
            int? durationReducedByHours,
            int? durationReducedBy,
            CourseDetails course)
        {
            EmployerAccountLegalEntityId = employerAccountLegalEntityId;
            EmployerName = employerName;
            TrainingProviderId = trainingProviderId;
            TrainingProviderName = trainingProviderName;
            DeliveryModel = deliveryModel;
            DurationReducedByHours = durationReducedByHours;
            DurationReducedBy = durationReducedBy;
            Course = course;
        }

        public long EmployerAccountLegalEntityId { get; }
        public string EmployerName { get; } = null!;

        public long TrainingProviderId { get; }
        public string TrainingProviderName { get; } = null!;
        public DeliveryModel DeliveryModel { get; }
        public CourseDetails Course { get; } = null!;
        public int? DurationReducedByHours { get; }
        public int? DurationReducedBy { get; }

        public bool EmployerIsEquivalent(ApprenticeshipDetails? other)
            => other != null &&
            EmployerAccountLegalEntityId == other.EmployerAccountLegalEntityId &&
            EmployerName == other.EmployerName;

        internal bool ProviderIsEquivalent(ApprenticeshipDetails other)
            => other != null &&
            TrainingProviderId == other.TrainingProviderId &&
            TrainingProviderName == other.TrainingProviderName;

        internal bool ApprenticeshipIsEquivalent(ApprenticeshipDetails other)
            => other != null &&
            Course.IsEquivalent(other.Course);

        internal bool DeliveryModelIsEquivalent(ApprenticeshipDetails other)
            => other != null &&
            DeliveryModel == other.DeliveryModel;

        public override bool Equals(object? obj) => obj switch
        {
            ApprenticeshipDetails other => Equals(other),
            _ => false,
        };

        public bool Equals(ApprenticeshipDetails? other) =>
            other != null &&
            other.EmployerAccountLegalEntityId == EmployerAccountLegalEntityId &&
            other.EmployerName == EmployerName &&
            other.TrainingProviderId == TrainingProviderId &&
            other.TrainingProviderName == TrainingProviderName &&
            other.DeliveryModel == DeliveryModel &&
            other.DurationReducedByHours == DurationReducedByHours &&
            other.DurationReducedBy == DurationReducedBy &&
            Course.IsEquivalent(other.Course);

        public override int GetHashCode() =>
            System.HashCode.Combine(
                EmployerAccountLegalEntityId,
                EmployerName,
                TrainingProviderId,
                TrainingProviderName,
                Course);
    }
}