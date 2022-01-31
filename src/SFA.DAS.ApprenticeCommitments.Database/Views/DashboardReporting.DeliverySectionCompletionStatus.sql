CREATE VIEW DashboardReporting.DeliverySectionCompletionStatus
AS    
select 
    'Delivery' AS Section,
    case when HowApprenticeshipDeliveredCorrect = 1 then 1 else null end as IsCorrect,
    case when HowApprenticeshipDeliveredCorrect is null then 1 else null end as IsIncomplete,
    case HowApprenticeshipDeliveredCorrect when 0 then 1 else null end as IsWrong,
	CommitmentsApprovedOn
from DashboardReporting.CommitmentStatementDashboardView
