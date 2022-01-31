CREATE VIEW DashboardReporting.EmployerSectionCompletionStatus
AS
select 
    'Employer' AS Section,
    case when EmployerCorrect = 1 then 1 else null end as IsConfirmed,
    case when EmployerCorrect is null then 1 else null end as IsIncomplete,
    case when EmployerCorrect = 0 then 1 else null end as IsWrong,
	CommitmentsApprovedOn
from DashboardReporting.CommitmentStatementDashboardView