using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.ApprenticeCommitments.Configuration;
using SFA.DAS.ApprenticeCommitments.Data;
using SFA.DAS.ApprenticeCommitments.Data.FuzzyMatching;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Application.Commands.VerifyRegistrationCommand
{
    public class VerifyRegistrationCommandHandler : IRequestHandler<VerifyRegistrationCommand>, IRequestHandler<VerifyRegistrationCommand2>
    {
        private readonly IRegistrationContext _registrations;
        private readonly IApprenticeContext _apprentices;
        private readonly ILogger<VerifyRegistrationCommandHandler> _logger;
        private readonly ApplicationSettings _applicationSettings;

        public VerifyRegistrationCommandHandler(
            IRegistrationContext registrations,
            IApprenticeContext apprenticeRepository,
            ILogger<VerifyRegistrationCommandHandler> logger,
            ApplicationSettings applicationSettings)
        {
            _registrations = registrations;
            _apprentices = apprenticeRepository;
            _logger = logger;
            _applicationSettings = applicationSettings;
        }

        public async Task<Unit> Handle(VerifyRegistrationCommand command, CancellationToken cancellationToken)
        {
            var registration = await _registrations.GetById(command.ApprenticeId);

            var matcher = new FuzzyMatcher(_applicationSettings.FuzzyMatchingSimilarityThreshold);

            if (!matcher.IsSimilar(registration.LastName, command.LastName))
            {
                _logger.LogInformation($"Verified Lastname ({command.LastName}) did not match registration {registration.ApprenticeId} ({registration.LastName})");
                throw new ValidationException(new[]
                {
                    new ValidationFailure("PersonalDetails", "Sorry, your identity has not been verified, please check your details"),
                });
            }

            if (registration.DateOfBirth.Date != command.DateOfBirth.Date)
            {
                _logger.LogInformation($"Verified DOB ({command.DateOfBirth}) did not match registration {registration.ApprenticeId} ({registration.DateOfBirth})");
                throw new ValidationException(new[]
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

        public async Task<Unit> Handle(VerifyRegistrationCommand2 request, CancellationToken cancellationToken)
        {
            var registration = await _registrations.GetById(request.RegistrationId);
            var apprentice = await _apprentices.GetById(request.ApprenticeId);

            if (registration.DateOfBirth.Date != apprentice.DateOfBirth.Date)
            {
                _logger.LogInformation($"Verified DOB ({apprentice.DateOfBirth.Date}) did not match registration {registration.ApprenticeId} ({registration.DateOfBirth.Date})");
                throw new IdentityNotVerifiedException();
            }

            return Unit.Value;
        }
    }

    public class IdentityNotVerifiedException : ValidationException
    {
        public IdentityNotVerifiedException()
            : base(new[]
            {
                new ValidationFailure("PersonalDetails", "Sorry, your identity has not been verified, please check your details"),
            })
        {
        }
    }
}