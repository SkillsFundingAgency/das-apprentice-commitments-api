CREATE VIEW DashboardReporting.ConfirmedCompletionStatus
AS    
select 
    'Confirmed' AS Section,
    case when ConfirmedOn is not null then 1 else null end as IsCorrect,
    case when ConfirmedOn is null then 1 else null end as IsIncomplete,
    NULL as IsWrong,
	CommitmentsApprovedOn
from DashboardReporting.CommitmentStatementDashboardView
