CREATE VIEW grafanaReporter.ApprenticeshipDashboardView
AS
SELECT        Id, ApprenticeId, CreatedOn
FROM          dbo.Apprenticeship
GO