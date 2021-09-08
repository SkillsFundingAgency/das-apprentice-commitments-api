﻿CREATE VIEW DashboardReporting.RegistrationDashboardView
AS
SELECT        [RegistrationId], CommitmentsApprenticeshipId, CommitmentsApprovedOn, CreatedOn, EmployerAccountLegalEntityId, EmployerName, TrainingProviderId, TrainingProviderName, CourseName, CourseLevel, CourseOption, PlannedStartDate, 
                         PlannedEndDate, FirstViewedOn, SignUpReminderSentOn
FROM          dbo.Registration
GO