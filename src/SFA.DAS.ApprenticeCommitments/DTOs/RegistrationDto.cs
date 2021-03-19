using System;

namespace SFA.DAS.ApprenticeCommitments.DTOs
{
    public class RegistrationDto
    {
        public Guid Id { get; set; }
        public long ApprenticeshipId { get; set; }
        public string Email { get; set; }
        public string EmployerName { get; set; }
        public long EmployerAccountLegalEntityId { get; set; }
        public long TrainingProviderId { get; internal set; }
        public string TrainingProviderName { get; internal set; }
        public DateTime? CreatedOn { get; private set; }
        public Guid? UserIdentityId { get; set; }
    }
}