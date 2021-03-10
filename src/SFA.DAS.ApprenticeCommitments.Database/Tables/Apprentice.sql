﻿CREATE TABLE [dbo].[Apprentice]
(
	[Id] uniqueidentifier NOT NULL,
	[FirstName] NVARCHAR(100) NOT NULL,
	[LastName] NVARCHAR(100) NOT NULL,
	[Email] NVARCHAR(200) NOT NULL, 
    [DateOfBirth] DATETIME2 NOT NULL,
	[CreatedOn] DATETIME2 NOT NULL DEFAULT current_timestamp, 
    CONSTRAINT PK_Apprentice_Id PRIMARY KEY ([Id])
)

GO
