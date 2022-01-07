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
            var result =
                CheckAlreadyCompletedByApprentice(apprenticeId)
                ?? EnsureNotAlreadyCompleted()
                ?? EnsureApprenticeDateOfBirthMatchesApproval(apprenticeId, dateOfBirth)
                ?? EnsureApprenticeNameMatchesApproval(apprenticeId, lastName, matcher)
                ?? AssociateWithApprentice(apprenticeId);

            MatchAttempts.Add(
                new ApprenticeshipMatchAttempt(RegistrationId, apprenticeId, result.Status));

            return result;
        }

        public IStatusResult<ApprenticeshipMatchAttemptStatus>? CheckAlreadyCompletedByApprentice(Guid apprenticeId) =>
            ApprenticeId == apprenticeId
                ? Result.SuccessStatus(ApprenticeshipMatchAttemptStatus.AlreadyCompleted)
                : null;

        private IStatusResult<ApprenticeshipMatchAttemptStatus>? EnsureNotAlreadyCompleted()
            => HasBeenCompleted
                ? Result.ExceptionStatus(
                    ApprenticeshipMatchAttemptStatus.AlreadyCompleted,
                    new RegistrationAlreadyMatchedException(RegistrationId))
                : null;

        private IStatusResult<ApprenticeshipMatchAttemptStatus>? EnsureApprenticeDateOfBirthMatchesApproval(Guid apprenticeId, DateTime dateOfBirth)
            => DateOfBirth.Date != dateOfBirth.Date
                ? Result.ExceptionStatus(
                    ApprenticeshipMatchAttemptStatus.MismatchedDateOfBirth,
                    new RegistrationMismatchDateOfBirthException(apprenticeId, RegistrationId))
                : default;

        private IStatusResult<ApprenticeshipMatchAttemptStatus>? EnsureApprenticeNameMatchesApproval(Guid apprenticeId, string lastName, FuzzyMatcher matcher)
            => !matcher.IsSimilar(LastName, lastName)
                ? Result.ExceptionStatus(
                    ApprenticeshipMatchAttemptStatus.MismatchedLastName,
                    new RegistrationMismatchLastNameException(apprenticeId, RegistrationId))
                : default;

        private SuccessStatusResult<ApprenticeshipMatchAttemptStatus> AssociateWithApprentice(Guid apprenticeId)
        {
            var apprenticeship = new Revision(
                                CommitmentsApprenticeshipId,
                                CommitmentsApprovedOn,
                                Approval);

            Apprenticeship = new Apprenticeship(apprenticeship, apprenticeId);
            ApprenticeId = apprenticeId;
            AddDomainEvent(new RegistrationMatched(Apprenticeship));

            return Result.SuccessStatus(ApprenticeshipMatchAttemptStatus.Succeeded);
        }

        public void ViewedByUser(DateTime viewedOn)
        {
            if (FirstViewedOn == null) FirstViewedOn = viewedOn;
        }

        public void SignUpReminderSent(DateTime sentOn)
        {
            if (SignUpReminderSentOn == null) SignUpReminderSentOn = sentOn;
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