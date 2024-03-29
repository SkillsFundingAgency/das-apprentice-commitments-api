﻿#nullable enable

using System;

namespace SFA.DAS.ApprenticeCommitments.Data.Models
{
    public class CourseDetails
    {
        private CourseDetails()
        {
            // Private constructor for entity framework
        }

        public CourseDetails(
            string name, int level, string? option,
            DateTime plannedStartDate, DateTime plannedEndDate,
            int courseDuration, DateTime? employmentEndDate)
        {
            Name = name;
            Level = level;
            Option = option;
            PlannedStartDate = plannedStartDate;
            PlannedEndDate = plannedEndDate;
            CourseDuration = courseDuration;
            EmploymentEndDate = employmentEndDate;
        }

        public string Name { get; private set; } = null!;
        public int Level { get; private set; }
        public string? Option { get; private set; }
        public DateTime PlannedStartDate { get; private set; }
        public DateTime PlannedEndDate { get; private set; }
        public DateTime? EmploymentEndDate { get; private set; }
        public int CourseDuration { get; private set; }

        public bool IsEquivalent(CourseDetails o)
        {
            if (o == null) throw new ArgumentNullException(nameof(o));
            return Name == o.Name && Level == o.Level && Option == o.Option &&
                PlannedStartDate == o.PlannedStartDate && PlannedEndDate == o.PlannedEndDate &&
                EmploymentEndDate == o.EmploymentEndDate;
        }
    }
}