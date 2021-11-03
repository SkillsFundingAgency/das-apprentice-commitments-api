using System;

namespace SFA.DAS.ApprenticeCommitments.Messages.Events
{
    public class ApprenticeshipEmailAddressChangedEvent
    {
        public Guid ApprenticeId { get; set; }
        public long CommitmentsApprenticeshipId { get; set; }
    }
}