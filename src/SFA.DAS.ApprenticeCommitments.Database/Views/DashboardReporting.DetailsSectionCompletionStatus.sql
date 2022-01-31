CREATE VIEW DashboardReporting.DetailsSectionCompletionStatus
AS    
select 
    'Details' AS Section,
    case when ApprenticeshipDetailsCorrect = 1 then 1 else null end as IsCorrect,
    case when ApprenticeshipDetailsCorrect is null then 1 else null end as IsIncomplete,
    case ApprenticeshipDetailsCorrect when 0 then 1 else null end as IsWrong,
	CommitmentsApprovedOn
from DashboardReporting.CommitmentStatementDashboardView
