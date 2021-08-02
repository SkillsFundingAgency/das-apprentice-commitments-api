CREATE VIEW grafanaReporter.ApprenticeshipDashboardView
AS
SELECT
	Id,
	ApprenticeId,
	CreatedOn,
	LastViewed
FROM
	dbo.Apprenticeship
GO