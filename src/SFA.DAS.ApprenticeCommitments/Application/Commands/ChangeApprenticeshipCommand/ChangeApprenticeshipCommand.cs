﻿using SFA.DAS.ApprenticeCommitments.Infrastructure.Mediator;
using System;

namespace SFA.DAS.ApprenticeCommitments.Application.Commands.ChangeApprenticeshipCommand
{
    public class ChangeApprenticeshipCommand : IUnitOfWorkCommand
    {
        public long? ContinuationOfCommitmentsApprenticeshipId { get; set; }
        public long CommitmentsApprenticeshipId { get; set; }
        public DateTime CommitmentsApprovedOn { get; set; }
        public string EmployerName { get; set; }
        public long EmployerAccountLegalEntityId { get; set; }
        public long TrainingProviderId { get; set; }
        public string TrainingProviderName { get; set; }
        public string CourseName { get; set; }
        public int CourseLevel { get; set; }
        public string CourseOption { get; set; }
        public DateTime PlannedStartDate { get; set; }
        public DateTime PlannedEndDate { get; set; }
    }
}