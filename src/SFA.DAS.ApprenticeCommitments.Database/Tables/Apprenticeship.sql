CREATE TABLE [dbo].[Apprenticeship]
(
    [UniqueId] BIGINT IDENTITY(1,1) NOT NULL, 
	[Id] BIGINT NOT NULL CONSTRAINT DF_Apprenticeship_Id default next value for ApprenticeshipIdNumbers,
	[ApprenticeId] UNIQUEIDENTIFIER NOT NULL, 
    [CommitmentsApprenticeshipId] BIGINT NOT NULL,
    [ApprovedOn] DATETIME2 NOT NULL DEFAULT GetUtcDate(), 
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
    CONSTRAINT PK_Apprenticeship_UniqueId PRIMARY KEY CLUSTERED ([UniqueId]),
    CONSTRAINT PK_Apprenticeship_ID_CommitmentsApprenticeshipId_CreatedOn UNIQUE ([Id], [CommitmentsApprenticeshipId], [ApprovedOn]),
	CONSTRAINT FK_Apprenticeship_ApprenticeId FOREIGN KEY ([ApprenticeId]) REFERENCES [dbo].[Apprentice] ([Id])
)

GO

CREATE NONCLUSTERED INDEX [IX_Apprenticeship_ApprenticeId] ON [dbo].[Apprenticeship]
(
	[ApprenticeId] ASC
)