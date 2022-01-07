using System;

namespace SFA.DAS.ApprenticeCommitments.Data.Models
{
    public class ApprenticeshipMatchAttempt
    {
        public long ApprenticeshipMatchAttemptId { get; private set; }
        public Guid RegistrationId { get; private set; }
        public Guid ApprenticeId { get; private set; }
        public ApprenticeshipMatchAttemptStatus Status { get; set; }

        private ApprenticeshipMatchAttempt()
        {
            // Entity Framework
        }

        public ApprenticeshipMatchAttempt(Guid registrationId, Guid apprenticeId)
        {
            RegistrationId = registrationId;
            ApprenticeId = apprenticeId;
        }
    }

    public enum ApprenticeshipMatchAttemptStatus
    {
        Succeeded,
        AlreadyCompleted,
        MismatchedDateOfBirth,
    }
}