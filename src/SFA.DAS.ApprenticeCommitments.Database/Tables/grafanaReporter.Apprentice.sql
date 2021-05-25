CREATE TABLE [grafanaReporter].[Apprentice]
(
	[Id] UNIQUEIDENTIFIER NOT NULL,
	[CreatedOn] DATETIME2 NOT NULL DEFAULT current_timestamp, 
    CONSTRAINT PK_Apprentice_Id PRIMARY KEY ([Id])
)

GO
