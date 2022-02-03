CREATE VIEW DashboardReporting.InvitationByStatus
AS
SELECT RegistrationId, Reg.CommitmentsApprenticeshipId, ConfirmBefore, ConfirmedOn, Rev.Id,
    CASE
        WHEN Rev.ConfirmedOn IS NOT NULL THEN 'Confirmed'
        WHEN Rev.ConfirmBefore < GETUTCDATE() THEN 'Started, overdue'
        WHEN Rev.Id IS NOT NULL THEN 'Started'
        WHEN Reg.CommitmentsApprovedOn < DATEADD(day, -14, GETUTCDATE()) THEN 'Not started, Overdue'
        ELSE 'Not started'
    END AS Status
FROM DashboardReporting.RegistrationDashboardView Reg
LEFT JOIN DashboardReporting.CommitmentStatementDashboardView Rev 
	ON Reg.CommitmentsApprenticeshipId = Rev.CommitmentsApprenticeshipId
          AND Rev.CommitmentsApprovedOn = (SELECT max(CommitmentsApprovedOn) 
                                           FROM DashboardReporting.CommitmentStatementDashboardView
                                           WHERE CommitmentsApprenticeshipId = Reg.CommitmentsApprenticeshipId)
