﻿CREATE VIEW grafanaReporter.CommitmentStatementDashboardView
AS
SELECT        Id, ApprenticeshipId, CommitmentsApprenticeshipId, CommitmentsApprovedOn, EmployerAccountLegalEntityId, EmployerName, TrainingProviderId, TrainingProviderName, CourseName, CourseLevel, PlannedStartDate, 
                         CourseOption, PlannedEndDate, TrainingProviderCorrect, EmployerCorrect, RolesAndResponsibilitiesCorrect, ApprenticeshipDetailsCorrect, HowApprenticeshipDeliveredCorrect, ApprenticeshipConfirmed
FROM          grafanaReporter.CommitmentStatement
GO