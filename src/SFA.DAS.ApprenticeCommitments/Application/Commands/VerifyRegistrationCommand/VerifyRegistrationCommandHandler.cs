using MediatR;
using SFA.DAS.ApprenticeCommitments.Data;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.ApprenticeCommitments.Exceptions;

namespace SFA.DAS.ApprenticeCommitments.Application.Commands.VerifyRegistrationCommand
{
    public class VerifyRegistrationCommandHandler : IRequestHandler<VerifyRegistrationCommand>
    {
        private readonly IRegistrationContext _registrations;
        private readonly IApprenticeContext _apprentices;

        public VerifyRegistrationCommandHandler(IRegistrationContext registrations, IApprenticeContext apprenticeRepository)
        {
            _registrations = registrations;
            _apprentices = apprenticeRepository;
        }

        public async Task<Unit> Handle(VerifyRegistrationCommand command, CancellationToken cancellationToken)
        {
            var registration = await _registrations.GetById(command.ApprenticeId);

            if (registration.DateOfBirth != command.DateOfBirth)
            {
                throw new DomainException("Sorry, your identity has not been verified, please check your details");
            }

            var apprentice = registration.ConvertToApprentice(
                command.FirstName, command.LastName,
                new MailAddress(command.Email), command.DateOfBirth,
                command.UserIdentityId);

            await _apprentices.AddAsync(apprentice);

            return Unit.Value;
        }
    }
}