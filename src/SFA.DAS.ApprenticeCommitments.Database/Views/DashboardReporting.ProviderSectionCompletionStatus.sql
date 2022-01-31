CREATE VIEW DashboardReporting.ProviderSectionCompletionStatus
AS    
select 
    'Provider' AS [Section],
    case when TrainingProviderCorrect = 1 then 1 else null end as IsCorrect,
    case when TrainingProviderCorrect is null then 1 else null end as IsIncomplete,
    case TrainingProviderCorrect when 0 then 1 else null end as IsWrong,
	CommitmentsApprovedOn
from DashboardReporting.CommitmentStatementDashboardView
