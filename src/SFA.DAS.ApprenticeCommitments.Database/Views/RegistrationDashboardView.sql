CREATE VIEW dbo.RegistrationDashboardView
AS
SELECT        ApprenticeId, ApprenticeshipId, ApprovedOn, Email, CreatedOn, EmployerAccountLegalEntityId, EmployerName, TrainingProviderId, TrainingProviderName, CourseName, CourseLevel, CourseOption, PlannedStartDate, 
                         PlannedEndDate, FirstViewedOn, SignUpReminderSentOn
FROM          dbo.Registration
GO