using System;

namespace SFA.DAS.ApprenticeCommitments.Messages.Events
{
    public class ApprenticeshipStoppedEvent
    {
        public long CommitmentsApprenticeshipId { get; set; }
        public long? ApprenticeshipId { get; set; }
        public Guid? ApprenticeId { get; set; }
        public string CourseName { get; set; }
        public string EmployerName { get; set; }
    }
}