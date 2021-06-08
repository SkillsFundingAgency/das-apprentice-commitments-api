#nullable enable

using System;
using static SFA.DAS.ApprenticeCommitments.Extensions.DateCalculations;

namespace SFA.DAS.ApprenticeCommitments.Data.Models
{
    public class CourseDetails
    {
#pragma warning disable CS8618 // Private constructor for entity framework

        private CourseDetails()
        {
        }

#pragma warning disable CS8618

        public CourseDetails(
            string name, int level, string? option,
            DateTime plannedStartDate, DateTime plannedEndDate)
        {
            Name = name;
            Level = level;
            Option = option;
            PlannedStartDate = plannedStartDate;
            PlannedEndDate = plannedEndDate;
        }

        public string Name { get; private set; }
        public int Level { get; private set; }
        public string? Option { get; private set; }
        public DateTime PlannedEndDate { get; private set; }
        public DateTime PlannedStartDate { get; private set; }

        public int DurationInMonths => DifferenceInMonths(PlannedStartDate, PlannedEndDate);

        public bool IsEquivalent(CourseDetails o)
        {
            if (o == null) throw new ArgumentNullException(nameof(o));
            return Name == o.Name && Level == o.Level && Option == o.Option &&
                PlannedStartDate == o.PlannedStartDate && PlannedEndDate == o.PlannedEndDate;
        }
    }
}