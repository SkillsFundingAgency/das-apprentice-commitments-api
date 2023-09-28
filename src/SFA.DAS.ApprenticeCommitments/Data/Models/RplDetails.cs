#nullable enable

using System;

namespace SFA.DAS.ApprenticeCommitments.Data.Models
{
    public class RplDetails
    {
        private RplDetails()
        {
            // Private constructor for entity framework
        }

        public RplDetails(bool? recognisePriorLearning, short? durationReducedByHours, short? durationReducedBy)
        {
            RecognisePriorLearning = recognisePriorLearning;
            DurationReducedByHours = durationReducedByHours;
            DurationReducedBy = durationReducedBy;
        }

        public bool? RecognisePriorLearning { get; }
        public short? DurationReducedByHours { get; }
        public short? DurationReducedBy { get; }

        public bool IsEquivalent(RplDetails o)
        {
            if (o == null) throw new ArgumentNullException(nameof(o));
            return RecognisePriorLearning == o.RecognisePriorLearning &&
                   DurationReducedByHours == o.DurationReducedByHours && DurationReducedBy == o.DurationReducedBy;
        }
    }
}