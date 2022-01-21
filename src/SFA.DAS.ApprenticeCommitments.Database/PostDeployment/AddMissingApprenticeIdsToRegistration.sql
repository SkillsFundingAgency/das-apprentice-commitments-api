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

    update Registration
    set ApprenticeshipId = 
        (select distinct ApprenticeshipId
         from Revision
         where Revision.CommitmentsApprenticeshipId = Registration.CommitmentsApprenticeshipId)
    where ApprenticeshipId is null
end
