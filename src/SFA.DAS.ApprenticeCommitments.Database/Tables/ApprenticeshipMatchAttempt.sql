CREATE TABLE [dbo].[ApprenticeshipMatchAttempt]
(
	[ApprenticeshipMatchAttemptId] bigint NOT NULL IDENTITY,
    [RegistrationId] uniqueidentifier NOT NULL,
    [ApprenticeId] uniqueidentifier NOT NULL,
    [Status] int NOT NULL,
    CONSTRAINT [PK_ApprenticeshipMatchAttempt] PRIMARY KEY ([ApprenticeshipMatchAttemptId]),
    CONSTRAINT [FK_ApprenticeshipMatchAttempt_Registration_RegistrationId] FOREIGN KEY ([RegistrationId]) REFERENCES [Registration] ([RegistrationId]) ON DELETE CASCADE
)
