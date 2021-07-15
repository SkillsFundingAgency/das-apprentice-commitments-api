using System;

namespace SFA.DAS.ApprenticeCommitments.Data.Models
{
    public struct PersonalInformation
    {
        public PersonalInformation(
            string firstName,
            string lastName,
            DateTime dateOfBirth)
        {
            FirstName = firstName;
            LastName = lastName;
            DateOfBirth = dateOfBirth;
        }

        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public DateTime DateOfBirth { get; private set; }
    }
}