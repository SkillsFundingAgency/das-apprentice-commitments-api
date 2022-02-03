CREATE VIEW DashboardReporting.MedianHoursBeforeActingOnInvite
AS
with Counts as
( 
    select
        datediff(hour, R.CommitmentsApprovedOn, A.CreatedOn) as HoursWaited
    from [DashboardReporting].[RegistrationDashboardView] R
    join [DashboardReporting].[ApprenticeshipDashboardView] A 
        on R.ApprenticeshipId = A.Id
)
select max(HoursWaited) AS HoursWaited
from (select top 50 percent HoursWaited from Counts order by HoursWaited) as w
