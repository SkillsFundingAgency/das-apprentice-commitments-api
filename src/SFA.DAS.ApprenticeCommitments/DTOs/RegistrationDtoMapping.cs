using SFA.DAS.ApprenticeCommitments.Data.Models;

namespace SFA.DAS.ApprenticeCommitments.DTOs
{
    public static class RegistrationDtoMapping
    {
        public static RegistrationDto MapToRegistrationDto(this Registration registration)
        {
            return new RegistrationDto
            {
                RegistrationId = registration.RegistrationId,
                CreatedOn = registration.CreatedOn,
                ApprenticeshipId = registration.CommitmentsApprenticeshipId,
                FirstName = registration.FirstName,
                LastName = registration.LastName,
                DateOfBirth = registration.DateOfBirth,
                Email = registration.Email.ToString(),
                EmployerName = registration.ApprenticeshipDetails.EmployerName,
                EmployerAccountLegalEntityId = registration.ApprenticeshipDetails.EmployerAccountLegalEntityId,
                UserIdentityId = registration.UserIdentityId,
                TrainingProviderId = registration.ApprenticeshipDetails.TrainingProviderId,
                TrainingProviderName = registration.ApprenticeshipDetails.TrainingProviderName,
                CourseName = registration.ApprenticeshipDetails.Course.Name,
            };
        }
    }
}