using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.ApprenticeCommitments.DTOs
{
    public class RevisionDto
    {
        public RevisionDto() { }

        public RevisionDto(string heading, string description, DateTime? revisionDate) =>
            (Heading, Description, RevisionDate) = (heading, description, revisionDate);

        public string Heading { get; set; } = "";
        public string Description { get; set; } = "";
        public DateTime? RevisionDate { get; set; }
    }
}
