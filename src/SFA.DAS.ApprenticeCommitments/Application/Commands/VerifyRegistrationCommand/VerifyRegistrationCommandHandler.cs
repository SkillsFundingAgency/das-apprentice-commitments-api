using MediatR;
using SFA.DAS.ApprenticeCommitments.Configuration;
using SFA.DAS.ApprenticeCommitments.Data;
using SFA.DAS.ApprenticeCommitments.Data.FuzzyMatching;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
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
                        
            var matcher = new FuzzyMatcher(_applicationSettings.SimilarityThreshold);

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