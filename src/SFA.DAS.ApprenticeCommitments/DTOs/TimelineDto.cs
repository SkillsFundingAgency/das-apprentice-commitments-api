using System;

namespace SFA.DAS.ApprenticeCommitments.DTOs
{
    public class TimelineDto
    {
        public TimelineDto() { }

        public TimelineDto(string heading, string description, DateTime? revisionDate) =>
            (Heading, Description, RevisionDate) = (heading, description, revisionDate);

        public string Heading { get; set; } = "";
        public string Description { get; set; } = "";
        public DateTime? RevisionDate { get; set; }
    }
}
