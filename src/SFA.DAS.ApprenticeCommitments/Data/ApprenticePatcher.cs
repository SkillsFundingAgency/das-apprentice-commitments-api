using System;
using System.Net.Mail;
using Microsoft.Extensions.Logging;
using SFA.DAS.ApprenticeCommitments.Data.Models;

namespace SFA.DAS.ApprenticeCommitments.Data
{
    public sealed class ApprenticePatcher
    {
        private readonly Apprentice _apprentice;
        private readonly ILogger _logger;

        public ApprenticePatcher(Apprentice apprentice, ILogger logger)
        {
            _apprentice = apprentice;
            _logger = logger;
        }

        public Guid ApprenticeId => _apprentice.Id;

        public string FirstName
        {
            set
            {
                _logger.LogInformation("Patching FirstName for Apprentice {id}", _apprentice.Id);
                _apprentice.FirstName = value;
            }
        }

        public string LastName
        {
            set
            {
                _logger.LogInformation("Patching LastName for Apprentice {id}", _apprentice.Id);
                _apprentice.LastName = value;
            }
        }

        public string Email
        {
            set
            {
                _logger.LogInformation("Patching Email for Apprentice {id}", _apprentice.Id);
                _apprentice.UpdateEmail(new MailAddress(value));
            }
        }

        public DateTime DateOfBirth
        {
            set
            {
                _logger.LogInformation("Patching DoB for Apprentice {id}", _apprentice.Id);
                _apprentice.DateOfBirth = value;
            }
        }

        public bool TermsOfUseAccepted
        {
            set
            {
                _logger.LogInformation("Patching TermsOfUse for Apprentice {id}", _apprentice.Id);
                _apprentice.TermsOfUseAccepted = value;
            }
        }
    }
}