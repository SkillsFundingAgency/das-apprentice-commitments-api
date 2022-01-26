CREATE TABLE [dbo].[ApprenticeshipMatchAttempt]
(
	[ApprenticeshipMatchAttemptId] bigint NOT NULL IDENTITY,
    [RegistrationId] uniqueidentifier NOT NULL,
    [ApprenticeId] uniqueidentifier NOT NULL,
    [Status] int NOT NULL,
    [AttemptedOn] DATETIME2 NOT NULL DEFAULT GetUtcDate(), 
    CONSTRAINT [PK_ApprenticeshipMatchAttempt] PRIMARY KEY ([ApprenticeshipMatchAttemptId]),
    CONSTRAINT [FK_ApprenticeshipMatchAttempt_Registration_RegistrationId] FOREIGN KEY ([RegistrationId]) REFERENCES [Registration] ([RegistrationId]) ON DELETE CASCADE
)
