CREATE TABLE [dbo].[Apprenticeship]
(
	[Id] BIGINT IDENTITY(10000,1) NOT NULL,
	[ApprenticeId] UNIQUEIDENTIFIER NOT NULL, 
    [CommitmentsApprenticeshipId] BIGINT NOT NULL,
	[EmployerAccountLegalEntityId] BIGINT NOT NULL,
	[EmployerName] NVARCHAR(100) NOT NULL, 
    [TrainingProviderId] BIGINT NOT NULL, 
    [TrainingProviderName] NVARCHAR(100) NOT NULL, 
    [CourseName] NVARCHAR(MAX) NOT NULL, 
    [CourseLevel] int NOT NULL, 
    [CourseOption] NVARCHAR(MAX) NULL, 
    [PlannedStartDate] datetime2 NOT NULL,
    [PlannedEndDate] datetime2 NOT NULL,
    [TrainingProviderCorrect] BIT NULL, 
    [EmployerCorrect] BIT NULL, 
    [RolesAndResponsibilitiesCorrect] BIT NULL, 
    [ApprenticeshipDetailsCorrect] bit NULL,
    [HowApprenticeshipDeliveredCorrect] BIT NULL, 
    [ApprenticeshipConfirmed] bit NULL,
    [CreatedOn] DATETIME2 NOT NULL DEFAULT GetUtcDate(), 
    CONSTRAINT PK_Apprenticeship_Id PRIMARY KEY CLUSTERED ([Id]),
	CONSTRAINT FK_Apprenticeship_ApprenticeId FOREIGN KEY ([ApprenticeId]) REFERENCES [dbo].[Apprentice] ([Id])
)

GO

CREATE NONCLUSTERED INDEX [IX_Apprenticeship_ApprenticeId] ON [dbo].[Apprenticeship]
(
	[ApprenticeId] ASC
)