﻿using SFA.DAS.ApprenticeCommitments.Application.DomainEvents;
using SFA.DAS.ApprenticeCommitments.Data.FuzzyMatching;
using SFA.DAS.ApprenticeCommitments.Exceptions;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Mail;

#nullable enable

namespace SFA.DAS.ApprenticeCommitments.Data.Models
{
    [Table("Registration")]
    public class Registration : Entity
    {
#pragma warning disable CS8618 // Private constructor for entity framework

        private Registration()
#pragma warning restore CS8618
        {
        }

        public Registration(
            Guid registrationId,
            long commitmentsApprenticeshipId,
            DateTime commitmentsApprovedOn,
            PersonalInformation pii,
            MailAddress email,
            ApprenticeshipDetails apprenticeship)
        {
            RegistrationId = registrationId;
            CommitmentsApprenticeshipId = commitmentsApprenticeshipId;
            CommitmentsApprovedOn = commitmentsApprovedOn;
            FirstName = pii.FirstName;
            LastName = pii.LastName;
            DateOfBirth = pii.DateOfBirth;
            Email = email;
            Apprenticeship = apprenticeship;

            AddDomainEvent(new RegistrationAdded(this));
        }

        public Guid RegistrationId { get; private set; }
        public long CommitmentsApprenticeshipId { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public DateTime DateOfBirth { get; private set; }
        public MailAddress Email { get; private set; }
        public Guid? UserIdentityId { get; private set; }
        public ApprenticeshipDetails Apprenticeship { get; private set; }
        public DateTime CommitmentsApprovedOn { get; private set; }
        public DateTime? CreatedOn { get; private set; } = DateTime.UtcNow;
        public DateTime? FirstViewedOn { get; private set; }
        public DateTime? SignUpReminderSentOn { get; private set; }

        public bool HasBeenCompleted => UserIdentityId != null;

        public void AssociateWithApprentice(Apprentice apprentice, FuzzyMatcher matcher)
        {
            EnsureNotAlreadyCompleted();
            EnsureApprenticeDateOfBirthMatchesApproval(apprentice.DateOfBirth);
            EnsureApprenticeNameMatchesApproval(apprentice, matcher);

            var apprenticeship = new Revision(
                    CommitmentsApprenticeshipId,
                    CommitmentsApprovedOn,
                    Apprenticeship);

            apprentice.AddApprenticeship(apprenticeship);
            UserIdentityId = apprentice.Id;
        }

        private void EnsureNotAlreadyCompleted()
        {
            if (HasBeenCompleted)
                throw new DomainException($"Registration {RegistrationId} is already verified");
        }

        private void EnsureApprenticeDateOfBirthMatchesApproval(DateTime dateOfBirth)
        {
            if (DateOfBirth.Date != dateOfBirth.Date)
            {
                throw new IdentityNotVerifiedException(
                    $"Verified DOB ({dateOfBirth.Date}) did not match registration {RegistrationId} ({DateOfBirth.Date})");
            }
        }
        private void EnsureApprenticeNameMatchesApproval(Apprentice apprentice, FuzzyMatcher matcher)
        {
            if (!matcher.IsSimilar(LastName, apprentice.LastName))
            {
                throw new IdentityNotVerifiedException(
                    $"Last name from account {apprentice.Id} did not match registration {RegistrationId}");
            }
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
            Apprenticeship = apprenticeshipDetails;
            FirstName = pii.FirstName;
            LastName = pii.LastName;
            DateOfBirth = pii.DateOfBirth;
        }
    }
}