using MediatR;
using SFA.DAS.ApprenticeCommitments.Data;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.ApprenticeCommitments.Exceptions;
using Microsoft.Extensions.Logging;
using FluentValidation;
using FluentValidation.Results;

namespace SFA.DAS.ApprenticeCommitments.Application.Commands.VerifyRegistrationCommand
{
    public class VerifyRegistrationCommandHandler : IRequestHandler<VerifyRegistrationCommand>
    {
        private readonly IRegistrationContext _registrations;
        private readonly IApprenticeContext _apprentices;
        private readonly ILogger<VerifyRegistrationCommandHandler> _logger;

        public VerifyRegistrationCommandHandler(IRegistrationContext registrations, IApprenticeContext apprenticeRepository, ILogger<VerifyRegistrationCommandHandler> logger)
        {
            _registrations = registrations;
            _apprentices = apprenticeRepository;
            _logger = logger;
        }

        public async Task<Unit> Handle(VerifyRegistrationCommand command, CancellationToken cancellationToken)
        {
            var registration = await _registrations.GetById(command.ApprenticeId);

            if (registration.DateOfBirth.Date != command.DateOfBirth.Date)
            {
                _logger.LogInformation($"Verified DOB ({command.DateOfBirth}) did not match registration {registration.ApprenticeId} ({registration.DateOfBirth})");
                throw new ValidationException(new []
                {
                    new ValidationFailure("PersonalDetails", "Sorry, your identity has not been verified, please check your details"),
                });
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