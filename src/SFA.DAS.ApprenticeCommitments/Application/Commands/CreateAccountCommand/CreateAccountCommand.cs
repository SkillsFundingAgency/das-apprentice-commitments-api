using SFA.DAS.ApprenticeCommitments.Infrastructure.Mediator;
using System;

namespace SFA.DAS.ApprenticeCommitments.Application.Commands.CreateAccountCommand
{
    public class CreateAccountCommand : IUnitOfWorkCommand
    {
        public Guid ApprenticeId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
}