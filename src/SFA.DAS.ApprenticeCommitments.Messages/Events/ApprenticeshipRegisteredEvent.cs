using System;

namespace SFA.DAS.ApprenticeCommitments.Messages.Events
{
    public class ApprenticeshipRegisteredEvent
    {
        public Guid RegistrationId { get; set; }
    }
}