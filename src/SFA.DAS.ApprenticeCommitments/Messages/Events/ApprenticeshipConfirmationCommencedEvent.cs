using System;

namespace SFA.DAS.ApprenticeCommitments.Messages.Events
{
    public class ApprenticeshipConfirmationCommencedEvent
    {
        public long ApprenticeshipId { get; set; }
        public long ConfirmationId { get; set; }
        public DateTime ConfirmationOverdueOn { get; set; }
        public long CommitmentsApprenticeshipId { get; set; }
        public DateTime CommitmentsApprovedOn { get; set; }
    }
}