﻿/*
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


/* Reporter Role Access */

IF DATABASE_PRINCIPAL_ID('Reporter') IS NULL
BEGIN
    CREATE ROLE [Reporter]
END

GRANT SELECT ON [grafanaReporter].ApprenticeDashboardView TO Reporter

GRANT SELECT ON [grafanaReporter].ApprenticeshipDashboardView TO Reporter

GRANT SELECT ON [grafanaReporter].CommitmentstatementDashboardView TO Reporter

GRANT SELECT ON [grafanaReporter].RegistrationDashboardView TO Reporter

alter table apprentice drop column lastviewed