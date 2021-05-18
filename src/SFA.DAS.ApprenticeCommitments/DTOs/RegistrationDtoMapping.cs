using SFA.DAS.ApprenticeCommitments.Data.Models;

namespace SFA.DAS.ApprenticeCommitments.DTOs
{
    public static class RegistrationDtoMapping
    {
        public static RegistrationDto MapToRegistrationDto(this Registration registration)
        {
            return new RegistrationDto
            {
                ApprenticeId = registration.ApprenticeId,
                CreatedOn = registration.CreatedOn,
                ApprenticeshipId = registration.CommitmentsApprenticeshipId,
                Email = registration.Email.ToString(),
                EmployerName = registration.Apprenticeship.EmployerName,
                EmployerAccountLegalEntityId = registration.Apprenticeship.EmployerAccountLegalEntityId,
                UserIdentityId = registration.UserIdentityId,
                TrainingProviderId = registration.Apprenticeship.TrainingProviderId,
                TrainingProviderName = registration.Apprenticeship.TrainingProviderName,
            };
        }
    }
}