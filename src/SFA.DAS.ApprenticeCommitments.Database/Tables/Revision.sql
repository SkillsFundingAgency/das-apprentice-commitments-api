CREATE TABLE [dbo].Revision
(
    [Id] BIGINT IDENTITY(1,1) NOT NULL, 
	[ApprenticeshipId] BIGINT NOT NULL,
    [CommitmentsApprenticeshipId] BIGINT NOT NULL,
    [CommitmentsApprovedOn] DATETIME2 NOT NULL, 
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
    [RolesAndResponsibilitiesConfirmations] TINYINT NULL, 
    [ApprenticeshipDetailsCorrect] bit NULL,
    [HowApprenticeshipDeliveredCorrect] BIT NULL, 
    [ConfirmBefore] DATETIME2 NOT NULL, 
    [ConfirmedOn] DATETIME2 NULL,
    [CourseDuration] INT NULL,
    [LastViewed] DATETIME2 NULL,
    [CreatedOn] DATETIME2 NULL DEFAULT current_timestamp, 
    [StoppedReceivedOn] DATETIME2 NULL, 
    [DeliveryModel] INT NOT NULL DEFAULT 0, 
    [EmploymentEndDate] DATETIME2 NULL, 
    [RecognisePriorLearning] BIT NULL,
    [DurationReducedByHours] SMALLINT NULL,
    [DurationReducedBy] SMALLINT NULL,
    CONSTRAINT PK_CommitmentStatement_Id PRIMARY KEY CLUSTERED ([Id]),
    CONSTRAINT FK_CommitmentStatement_ApprenticeshipId FOREIGN KEY ([ApprenticeshipId]) REFERENCES [Apprenticeship] ([Id])
)

GO

CREATE INDEX [IX_CommitmentStatement_ApprenticeshipId] ON Revision ([ApprenticeshipId]);
GO
CREATE INDEX [IX_CommitmentStatement_CommitmentsApprenticeshipId] ON Revision ([CommitmentsApprenticeshipId]);
GO