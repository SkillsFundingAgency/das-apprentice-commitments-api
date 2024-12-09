/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

/* Reporter Role Access

IF DATABASE_PRINCIPAL_ID('Reporter') IS NULL
BEGIN
    CREATE ROLE [Reporter]
END

GRANT SELECT ON [DashboardReporting].ApprenticeshipDashboardView TO Reporter

GRANT SELECT ON [DashboardReporting].CommitmentstatementDashboardView TO Reporter

GRANT SELECT ON [DashboardReporting].RegistrationDashboardView TO Reporter

-- ****************** Remove Apprentice dashboard reporting view ******************

if exists(select 1 from sys.views where name='ApprenticeDashboardView' and type='v')
    drop view [DashboardReporting].ApprenticeDashboardView;
go

-- ****************** Remove Apprentice dashboard reporting view ******************


-- ****************** AddMissingApprenticeIdsToRegistration.sql ******************

-- This is only Registrations that are missing the ApprenticeshipId
--select count(*) from registration where apprenticeshipid is null and commitmentsApprenticeshipId in (select commitmentsApprenticeshipId from revision)

if exists(
    select * from Registration
    where ApprenticeshipId is null
        and CommitmentsApprenticeshipId in (select CommitmentsApprenticeshipId from Revision))
begin
    print 'Adding missing Apprenticeship IDs to Registrations'

    -- See if there are any Registrations that have more than one CommitmentsApprenticeshipId in a Revision
    -- THIS SHOULD RETURN ZERO RESULTS
    if exists (
        select top 1000 RegistrationId, count(distinct Revision.ApprenticeshipId) NumApprs
        from Registration inner join Revision on Registration.CommitmentsApprenticeshipId = Revision.CommitmentsApprenticeshipId
        group by RegistrationId
        having count(distinct Revision.ApprenticeshipId) > 1
    )
    begin
        raiserror (N'There are multiple apprentices for a registration', 16, 127) with nowait
    end
    else
    begin
    
        update Registration
        set ApprenticeshipId = 
            (select distinct ApprenticeshipId
             from Revision
             where Revision.CommitmentsApprenticeshipId = Registration.CommitmentsApprenticeshipId)
        where ApprenticeshipId is null
    
    end
end

-- ****************** AddMissingApprenticeIdsToRegistration.sql ******************

*/
