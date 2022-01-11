using System;

#nullable disable

namespace SFA.DAS.ApprenticeCommitments.Application.Queries.ApprovalsRegistrationQuery
{
    public class ApprovalsRegistrationResponse
    {
        public Guid RegistrationId { get; set; }
        public string Email { get; set; }
        public bool HasApprenticeAssigned { get; set; }
    }
}