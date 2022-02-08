using System;

namespace SFA.DAS.ApprenticeCommitments.Messages.Commands
{
    public class SendApprenticeshipInvitationCommand
    {
        public long CommitmentsApprenticeshipId { get; set; }
        public DateTime ResendOn { get; set; }
    }
}
