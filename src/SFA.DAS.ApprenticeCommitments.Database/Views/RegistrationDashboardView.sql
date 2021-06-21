CREATE VIEW grafanaReporter.RegistrationDashboardView
AS
SELECT        ApprenticeId, CommitmentsApprenticeshipId, CommitmentsApprovedOn, CreatedOn, EmployerAccountLegalEntityId, EmployerName, TrainingProviderId, TrainingProviderName, CourseName, CourseLevel, CourseOption, PlannedStartDate, 
                         PlannedEndDate, FirstViewedOn, SignUpReminderSentOn
FROM          dbo.Registration
GO