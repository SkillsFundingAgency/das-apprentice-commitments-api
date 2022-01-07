using SFA.DAS.ApprenticeCommitments.Application.DomainEvents;
using SFA.DAS.ApprenticeCommitments.Data.FuzzyMatching;
using SFA.DAS.ApprenticeCommitments.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Mail;

#nullable enable

namespace SFA.DAS.ApprenticeCommitments.Data.Models
{
    [Table("Registration")]
    public class Registration : Entity
    {
        private Registration()
        {
            // Private constructor for entity framework
        }

        public Registration(
            Guid registrationId,
            long commitmentsApprenticeshipId,
            DateTime commitmentsApprovedOn,
            PersonalInformation pii,
            ApprenticeshipDetails apprenticeship)
        {
            RegistrationId = registrationId;
            CommitmentsApprenticeshipId = commitmentsApprenticeshipId;
            CommitmentsApprovedOn = commitmentsApprovedOn;
            FirstName = pii.FirstName;
            LastName = pii.LastName;
            DateOfBirth = pii.DateOfBirth;
            Email = pii.Email;
            Approval = apprenticeship;

            AddDomainEvent(new RegistrationAdded(this));
        }

        public Guid RegistrationId { get; private set; }
        public long CommitmentsApprenticeshipId { get; private set; }
        public string FirstName { get; private set; } = null!;
        public string LastName { get; private set; } = null!;
        public DateTime DateOfBirth { get; private set; }
        public MailAddress Email { get; private set; } = null!;
        public Guid? ApprenticeId { get; private set; }
        public ApprenticeshipDetails Approval { get; private set; } = null!;
        public DateTime CommitmentsApprovedOn { get; private set; }
        public DateTime? CreatedOn { get; private set; } = DateTime.UtcNow;
        public DateTime? FirstViewedOn { get; private set; }
        public DateTime? SignUpReminderSentOn { get; private set; }
        public Apprenticeship? Apprenticeship { get; private set; }

        public List<ApprenticeshipMatchAttempt> MatchAttempts = new List<ApprenticeshipMatchAttempt>();

        public bool HasBeenCompleted => ApprenticeId != null;

        public IResult AssociateWithApprentice(Guid apprenticeId, string lastName, DateTime dateOfBirth, FuzzyMatcher matcher)
        {
            var attempt = new ApprenticeshipMatchAttempt(RegistrationId, apprenticeId);
            MatchAttempts.Add(attempt);

            var result =
                AlreadyCompletedByApprentice(attempt, apprenticeId)
                ?? EnsureNotAlreadyCompleted(attempt)
                ?? EnsureApprenticeDateOfBirthMatchesApproval(attempt, apprenticeId, dateOfBirth)
                ?? EnsureApprenticeNameMatchesApproval(apprenticeId, lastName, matcher);

            if(result is IStatusResult<ApprenticeshipMatchAttemptStatus> r2)
            {
                attempt.Status = r2.Status;
                return result;
            }

            var apprenticeship = new Revision(
                    CommitmentsApprenticeshipId,
                    CommitmentsApprovedOn,
                    Approval);

            Apprenticeship = new Apprenticeship(apprenticeship, apprenticeId);
            ApprenticeId = apprenticeId;
            AddDomainEvent(new RegistrationMatched(Apprenticeship));

            return new SuccessResult();
        }

        public IStatusResult<ApprenticeshipMatchAttemptStatus>? AlreadyCompletedByApprentice(ApprenticeshipMatchAttempt attempt, Guid apprenticeId)
        {
            if (ApprenticeId == apprenticeId)
            {
                return ResultX.SuccessStatus(ApprenticeshipMatchAttemptStatus.AlreadyCompleted);
            }

            return null;
        }

        private IResult? EnsureNotAlreadyCompleted(ApprenticeshipMatchAttempt attempt)
        {
            if (HasBeenCompleted)
            {
                return ResultX.ExceptionStatus(
                    ApprenticeshipMatchAttemptStatus.AlreadyCompleted, 
                    new RegistrationAlreadyMatchedException(RegistrationId));
            }

            return default;
        }

        private IResult? EnsureApprenticeDateOfBirthMatchesApproval(ApprenticeshipMatchAttempt attempt, Guid apprenticeId, DateTime dateOfBirth)
        {
            if (DateOfBirth.Date != dateOfBirth.Date)
            {
                return ResultX.ExceptionStatus(
                    ApprenticeshipMatchAttemptStatus.MismatchedDateOfBirth,
                    new IdentityNotVerifiedException(
                        $"DoB ({dateOfBirth.Date}) from account {apprenticeId} did not match registration {RegistrationId} ({DateOfBirth.Date})"));
            }

            return default;
        }

        private IResult? EnsureApprenticeNameMatchesApproval(Guid apprenticeId, string lastName, FuzzyMatcher matcher)
        {
            if (!matcher.IsSimilar(LastName, lastName))
            {
                return ResultX.ExceptionStatus(
                    ApprenticeshipMatchAttemptStatus.AlreadyCompleted,
                    new IdentityNotVerifiedException(
                        $"Last name from account {apprenticeId} did not match registration {RegistrationId}"));
            }

            return default;
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

        public void RenewApprenticeship(long commitmentsApprenticeshipId, DateTime commitmentsApprovedOn, ApprenticeshipDetails apprenticeshipDetails, PersonalInformation pii)
        {
            if (HasBeenCompleted)
            {
                throw new DomainException("Cannot update registration as user has confirmed their identity");
            }

            CommitmentsApprenticeshipId = commitmentsApprenticeshipId;
            CommitmentsApprovedOn = commitmentsApprovedOn;
            Approval = apprenticeshipDetails;
            FirstName = pii.FirstName;
            LastName = pii.LastName;
            DateOfBirth = pii.DateOfBirth;
            Email = pii.Email;

            DomainEvents.Add(new RegistrationUpdated(this));
        }
    }
}