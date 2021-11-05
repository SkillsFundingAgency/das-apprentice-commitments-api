using FluentValidation;
using SFA.DAS.ApprenticeCommitments.Exceptions;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using SFA.DAS.ApprenticeCommitments.Application.DomainEvents;

namespace SFA.DAS.ApprenticeCommitments.Data.Models
{
    public class Apprentice : Entity
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
        }

        public Guid Id { get; private set; }

        public void AddApprenticeship(Revision apprenticeship)
        {
            Apprenticeships.Add(new Apprenticeship(apprenticeship));
        }

        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;

        private MailAddress _email = null!;
        public MailAddress Email
        {
            get => _email;
            set
            {
                if (_email != null)
                {
                    if (value.Address == _email.Address)
                        return;
                    else
                        AddDomainEvent(new ApprenticeEmailAddressChanged(this));
                }

                _email = value;
                PreviousEmailAddresses.Add(new ApprenticeEmailAddressHistory(_email));
            }
        }

        public ICollection<ApprenticeEmailAddressHistory> PreviousEmailAddresses { get; private set; } = new List<ApprenticeEmailAddressHistory>();

        private DateTime _dateOfBirth;

        public DateTime DateOfBirth
        {
            get => _dateOfBirth;
            set
            {
                // domain validation to go here
                if (false) throw new DomainException("validation error");
                _dateOfBirth = value;
            }
        }

        private DateTime? _termsOfUseAcceptedOn;

        public bool TermsOfUseAccepted
        {
            get => _termsOfUseAcceptedOn != null;
            set
            {
                if (!value) throw new ValidationException("Cannot decline the Terms of Use");
                _termsOfUseAcceptedOn = DateTime.Now;
            }
        }

        public ICollection<Apprenticeship> Apprenticeships { get; private set; }
                    = new List<Apprenticeship>();

        public DateTime CreatedOn { get; private set; } = DateTime.UtcNow;
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