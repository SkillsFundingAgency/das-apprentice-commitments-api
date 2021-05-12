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


/* Developer Role Access */

IF DATABASE_PRINCIPAL_ID('Developer') IS NULL
BEGIN
    CREATE ROLE [Developer]
END

GRANT SELECT ON ApprenticeDashboardView TO Developer

GRANT SELECT ON ApprenticeshipDashboardView TO Developer

GRANT SELECT ON RegistrationDashboardView TO Developer
