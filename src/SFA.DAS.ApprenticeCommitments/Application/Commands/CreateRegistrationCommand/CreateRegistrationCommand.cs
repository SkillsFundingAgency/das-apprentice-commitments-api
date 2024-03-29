﻿using SFA.DAS.ApprenticeCommitments.Data.Models;
using SFA.DAS.ApprenticeCommitments.Infrastructure.Mediator;
using System;

#nullable disable

namespace SFA.DAS.ApprenticeCommitments.Application.Commands.CreateRegistrationCommand
{
    public class CreateRegistrationCommand : IUnitOfWorkCommand
    {
        public Guid RegistrationId { get; set; }
        public long CommitmentsApprenticeshipId { get; set; }
        public DateTime CommitmentsApprovedOn { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Email { get; set; }
        public string EmployerName { get; set; }
        public long EmployerAccountLegalEntityId { get; set; }
        public long TrainingProviderId { get; set; }
        public string TrainingProviderName { get; set; }
        public DeliveryModel DeliveryModel { get; set; }
        public string CourseName { get; set; }
        public int CourseLevel { get; set; }
        public int CourseDuration { get; set; }
        public string CourseOption { get; set; }
        public DateTime PlannedStartDate { get; set; }
        public DateTime PlannedEndDate { get; set; }
        public DateTime? EmploymentEndDate { get; set; }
        public bool? RecognisePriorLearning { get; set; }
        public short? DurationReducedByHours { get; set; }
        public short? DurationReducedBy { get; set; }

    }
}