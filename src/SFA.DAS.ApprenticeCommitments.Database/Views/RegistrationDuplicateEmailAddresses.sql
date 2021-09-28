create view DashboardReporting.RegistrationDuplicateEmailAddresses
as
select 
	count(*)/2 as DuplicateCount
from 
	Registration a 
	left join Registration b on a.Email = b.Email and a.ApprenticeId <> b.ApprenticeId
where
	a.email = b.email
