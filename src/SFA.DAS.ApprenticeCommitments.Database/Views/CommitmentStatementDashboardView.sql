CREATE VIEW dbo.CommitmentStatementDashboardView
AS
SELECT        Id, ApprenticeshipId, CommitmentsApprenticeshipId, CommitmentsApprovedOn, EmployerAccountLegalEntityId, EmployerName, TrainingProviderId, TrainingProviderName, CourseName, CourseLevel, PlannedStartDate, 
                         CourseOption, PlannedEndDate, TrainingProviderCorrect, EmployerCorrect, RolesAndResponsibilitiesCorrect, ApprenticeshipDetailsCorrect, HowApprenticeshipDeliveredCorrect, ApprenticeshipConfirmed
FROM          dbo.CommitmentStatement
GO