CREATE VIEW DashboardReporting.HoursBeforeActingOnInvite
AS
select
  case
    when datediff(hour, CommitmentsApprovedOn, A.CreatedOn) > 300 then 300
    else datediff(hour, CommitmentsApprovedOn, A.CreatedOn)
  end as HoursWaited,
  R.CommitmentsApprovedOn
from [DashboardReporting].[RegistrationDashboardView] R
join [DashboardReporting].[ApprenticeshipDashboardView] A on R.ApprenticeshipId = A.Id
