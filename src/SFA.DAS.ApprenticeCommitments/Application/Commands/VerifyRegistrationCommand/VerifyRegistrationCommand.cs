using SFA.DAS.ApprenticeCommitments.Infrastructure.Mediator;
using System;

#nullable disable

namespace SFA.DAS.ApprenticeCommitments.Application.Commands.VerifyRegistrationCommand
{
    public class VerifyRegistrationCommand : IUnitOfWorkCommand
    {
        public Guid ApprenticeId { get; set; }
        public Guid UserIdentityId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
}