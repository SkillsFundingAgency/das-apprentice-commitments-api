using System;
using System.Net.Mail;

#nullable disable

namespace SFA.DAS.ApprenticeCommitments.DTOs
{
    public class ApprenticeDto
    {
        public long? Id { get; internal set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Guid UserIdentityId { get; set; }
        public MailAddress Email { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
}
