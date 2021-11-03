using System;
using SFA.DAS.ApprenticeCommitments.Infrastructure.Mediator;

#nullable disable

namespace SFA.DAS.ApprenticeCommitments.Application.Commands.ChangeEmailAddressCommand
{
    public class ChangeEmailAddressCommand : IUnitOfWorkCommand
    {
        public ChangeEmailAddressCommand(Guid apprenticeId, string email)
        {
            ApprenticeId = apprenticeId;
            Email = email;
        }

        public Guid ApprenticeId { get; }
        public string Email { get; }
    }
}