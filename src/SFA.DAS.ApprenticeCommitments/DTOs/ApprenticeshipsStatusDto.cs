#nullable disable

namespace SFA.DAS.ApprenticeCommitments.DTOs
{
    public class ApprenticeshipStatusDto
    {
        public long ApprenticeshipId { get; set; }
        public bool HasBeenConfirmedAtLeastOnce { get; set; }
    }
}