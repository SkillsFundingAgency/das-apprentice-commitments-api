﻿using System;

#nullable disable

namespace SFA.DAS.ApprenticeCommitments.DTOs
{
    public class ApprenticeDto
    {
        public Guid Id { get; internal set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Guid UserIdentityId { get; set; }
        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
}
