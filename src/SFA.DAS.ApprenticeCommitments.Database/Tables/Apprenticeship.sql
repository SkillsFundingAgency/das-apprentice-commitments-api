CREATE TABLE [dbo].[Apprenticeship]
(
	[Id] BIGINT NOT NULL IDENTITY,
	[ApprenticeId] UNIQUEIDENTIFIER NOT NULL,
	[CreatedOn] DATETIME2 NOT NULL DEFAULT current_timestamp, 
    CONSTRAINT PK_Apprenticeship_Id PRIMARY KEY ([Id]), 
)

GO

CREATE INDEX [IX_Apprenticeship_ApprenticeId] ON [Apprenticeship] ([ApprenticeId]);
GO