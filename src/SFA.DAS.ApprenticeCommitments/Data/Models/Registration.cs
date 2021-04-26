using SFA.DAS.ApprenticeCommitments.Exceptions;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Mail;

#nullable enable

namespace SFA.DAS.ApprenticeCommitments.Data.Models
{
    [Table("Registration")]
    public class Registration
    {
#pragma warning disable CS8618 // Private constructor for entity framework

        private Registration()
#pragma warning restore CS8618
        {
        }

        public Registration(
            Guid apprenticeId,
            long apprenticeshipId,
            DateTime approvedOn,
            MailAddress email,
            ApprenticeshipDetails apprenticeship)
        {
            ApprenticeId = apprenticeId;
            ApprenticeshipId = apprenticeshipId;
            ApprovedOn = approvedOn;
            Email = email;
            Apprenticeship = apprenticeship;
        }

        public Guid ApprenticeId { get; private set; }
        public long ApprenticeshipId { get; private set; }
        public MailAddress Email { get; private set; }
        public Guid? UserIdentityId { get; private set; }
        public ApprenticeshipDetails Apprenticeship { get; private set; }
        public DateTime ApprovedOn { get; private set; }
        public DateTime? CreatedOn { get; private set; } = DateTime.UtcNow;
        public DateTime? FirstViewedOn { get; private set; }
        public DateTime? SignUpReminderSentOn { get; private set; }

        public bool HasBeenCompleted => UserIdentityId != null;

        public Apprentice ConvertToApprentice(string firstName, string lastName, MailAddress emailAddress, DateTime dateOfBirth, Guid userIdentityId)
        {
            EnsureNotAlreadyCompleted();
            EnsureStatedEmailMatchesApproval(emailAddress);

            UserIdentityId = userIdentityId;
            return CreateRegisteredApprentice(firstName, lastName, emailAddress, dateOfBirth);
        }

        public void ViewedByUser(DateTime viewedOn)
        {
            if (FirstViewedOn.HasValue)
            {
                return;
            }

            FirstViewedOn = viewedOn;
        }

        public void SignUpReminderSent(DateTime sentOn)
        {
            if (SignUpReminderSentOn.HasValue)
            {
                return;
            }

            SignUpReminderSentOn = sentOn;
        }


        private void EnsureNotAlreadyCompleted()
        {
            if (HasBeenCompleted)
                throw new DomainException($"Registration {ApprenticeId} id already verified");
        }

        private void EnsureStatedEmailMatchesApproval(MailAddress emailAddress)
        {
            if (!emailAddress.ToString().Equals(Email.ToString(), StringComparison.InvariantCultureIgnoreCase))
                throw new DomainException($"Email from verifying user doesn't match registered user {ApprenticeId}");
        }

        private Apprentice CreateRegisteredApprentice(string firstName, string lastName, MailAddress emailAddress, DateTime dateOfBirth)
        {
            var apprentice = new Apprentice(
                ApprenticeId, firstName, lastName, emailAddress, dateOfBirth);

            apprentice.AddApprenticeship(new Apprenticeship(ApprenticeshipId, ApprovedOn, Apprenticeship));

            return apprentice;
        }
    }
}