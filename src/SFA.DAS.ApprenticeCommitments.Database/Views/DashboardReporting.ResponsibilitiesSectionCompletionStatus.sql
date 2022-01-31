CREATE VIEW DashboardReporting.ResponsibilitiesSectionCompletionStatus
AS    
select 
    'Responsibilities' AS Section,
    case when RolesAndResponsibilitiesConfirmations = 7 then 1 else null end as IsCorrect,
    case when ISNULL(RolesAndResponsibilitiesConfirmations,0) < 7 then 1 else null end as IsIncomplete,
    NULL as IsWrong,
	CommitmentsApprovedOn
from DashboardReporting.CommitmentStatementDashboardView
