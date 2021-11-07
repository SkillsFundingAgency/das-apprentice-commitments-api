using System;

#nullable disable

namespace SFA.DAS.ApprenticeCommitments.DTOs
{
    public class ApprenticeUpdateDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }
        public bool TermsOfUseAccepted { get; set; }
    }
}
