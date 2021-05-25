CREATE VIEW grafanaReporter.ApprenticeshipDashboardView
AS
SELECT        Id, ApprenticeId, CreatedOn
FROM          grafanaReporter.Apprenticeship
GO