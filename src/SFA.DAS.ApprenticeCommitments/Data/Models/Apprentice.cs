﻿using System;
using System.Collections.Generic;
using System.Net.Mail;

namespace SFA.DAS.ApprenticeCommitments.Data.Models
{
    public class Apprentice
    {
        private Apprentice()
        {
            // for Entity Framework
        }

        public Apprentice(Guid Id, string firstName, string lastName, MailAddress email, DateTime dateOfBirth)
        {
            this.Id = Id;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            DateOfBirth = dateOfBirth;
            PreviousEmailAddresses = new[] { new ApprenticeEmailAddressHistory(email) };
        }

        public Guid Id { get; private set; }

        public void AddApprenticeship(CommitmentStatement apprenticeship)
        {
            Apprenticeships.Add(new Apprenticeship(apprenticeship));
        }

        public string FirstName { get; private set; } = null!;
        public string LastName { get; private set; } = null!;
        public MailAddress Email { get; private set; } = null!;
        public ICollection<ApprenticeEmailAddressHistory> PreviousEmailAddresses { get; private set; } = null!;
        public DateTime DateOfBirth { get; private set; }

        public ICollection<Apprenticeship> Apprenticeships { get; private set; }
            = new List<Apprenticeship>();

        public DateTime CreatedOn { get; private set; } = DateTime.UtcNow;

        internal void UpdateEmail(MailAddress newEmail)
        {
            if (newEmail.Address == Email.Address) return;
            Email = newEmail;
            PreviousEmailAddresses.Add(new ApprenticeEmailAddressHistory(Email));
        }
    }

    public class ApprenticeEmailAddressHistory
    {
        private ApprenticeEmailAddressHistory()
        {
        }

        public ApprenticeEmailAddressHistory(MailAddress emailAddress)
            => EmailAddress = emailAddress;

        public MailAddress EmailAddress { get; private set; } = null!;
        public DateTime ChangedOn { get; private set; } = DateTime.UtcNow;
    }
}