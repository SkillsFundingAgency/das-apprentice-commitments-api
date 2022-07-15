CREATE TABLE [dbo].Obsolete_ApprenticeEmailAddressHistory_DONOTUSE (
    [Id]           BIGINT            IDENTITY (2000, 1) NOT NULL,
    [EmailAddress] NVARCHAR (MAX) NULL,
    [ChangedOn]    DATETIME2 (7)  NOT NULL,
    [ApprenticeId] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_ApprenticeEmailAddressHistory] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ApprenticeEmailAddressHistory_Apprentice_ApprenticeId] FOREIGN KEY ([ApprenticeId]) REFERENCES [dbo].Obsolete_Apprentice_DONOTUSE ([Id]) ON DELETE CASCADE
);

GO
CREATE NONCLUSTERED INDEX [IX_ApprenticeEmailAddressHistory_ApprenticeId]
    ON [dbo].Obsolete_ApprenticeEmailAddressHistory_DONOTUSE([ApprenticeId] ASC);
