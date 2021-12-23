CREATE VIEW DashboardReporting.ApprenticeshipDashboardView
AS
SELECT
	Id,
	ApprenticeId,
	CreatedOn
FROM
	dbo.Apprenticeship
GO