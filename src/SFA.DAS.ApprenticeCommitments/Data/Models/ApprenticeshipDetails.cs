#nullable enable

namespace SFA.DAS.ApprenticeCommitments.Data.Models
{
    public class ApprenticeshipDetails
    {
#pragma warning disable CS8618 // Private constructor for entity framework
        private ApprenticeshipDetails()
        {
        }
#pragma warning disable CS8618

        public ApprenticeshipDetails(
            long employerAccountLegalEntityId, string employerName,
            long trainingProviderId, string trainingProviderName,
            CourseDetails course)
        {
            EmployerAccountLegalEntityId = employerAccountLegalEntityId;
            EmployerName = employerName;
            TrainingProviderId = trainingProviderId;
            TrainingProviderName = trainingProviderName;
            Course = course;
        }

        public long EmployerAccountLegalEntityId { get; private set; }
        public string EmployerName { get; private set; }

        public long TrainingProviderId { get; private set; }
        public string TrainingProviderName { get; private set; }
        public CourseDetails Course { get; private set; }
    }
}