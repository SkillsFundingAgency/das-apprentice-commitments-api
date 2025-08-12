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
                EmployerName = registration.Approval.EmployerName,
                EmployerAccountLegalEntityId = registration.Approval.EmployerAccountLegalEntityId,
                UserIdentityId = registration.ApprenticeId,
                TrainingProviderId = registration.Approval.TrainingProviderId,
                TrainingProviderName = registration.Approval.TrainingProviderName,
                CourseName = registration.Approval.Course.Name,
                StoppedReceivedOn = registration.StoppedReceivedOn,
                ApprenticeId = registration.ApprenticeId,
                ApprenticeshipType = registration.Approval.ApprenticeshipType
            };
        }
    }
}