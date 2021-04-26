using SFA.DAS.ApprenticeCommitments.Infrastructure.Mediator;
using System;

namespace SFA.DAS.ApprenticeCommitments.Application.Commands.CreateRegistrationCommand
{
    public class CreateRegistrationCommand : IUnitOfWorkCommand
    {
        public Guid ApprenticeId { get; set; }
        public long ApprenticeshipId { get; set; }
        public DateTime ApprovedOn { get; set; }
        public string Email { get; set; }
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