/*
RUN THIS AFTER THE G&S UPDATES HAVE BEEN DEPLOYED AND [REVISION] AND [REGISTRATION] TABLE CONTAINS NEW FIELD APPRENTICESHIP TYPE
*/

USE [SFA.DAS.ApprenticeCommitments.Database]
GO

BEGIN TRY
    -- Check if tables and columns exist
    IF NOT EXISTS (
        SELECT 1 
        FROM INFORMATION_SCHEMA.COLUMNS 
        WHERE TABLE_NAME = 'Revision' 
        AND COLUMN_NAME = 'ApprenticeshipType'
    )
    BEGIN
        RAISERROR('Revision table or ApprenticeshipType column does not exist', 16, 1)
    END

    IF NOT EXISTS (
        SELECT 1 
        FROM INFORMATION_SCHEMA.COLUMNS 
        WHERE TABLE_NAME = 'Registration' 
        AND COLUMN_NAME = 'ApprenticeshipType'
    )
    BEGIN
        RAISERROR('Registration table or ApprenticeshipType column does not exist', 16, 1)
    END

    BEGIN TRANSACTION

    -- Backup current values (safety precaution)
    SELECT * INTO #Revision_Backup FROM [Revision]
    SELECT * INTO #Registration_Backup FROM [Registration]

    -- Perform updates
    UPDATE [Revision] 
    SET [ApprenticeshipType] = 0
    WHERE [ApprenticeshipType] IS NOT NULL  -- Skip NULL values if any
    
    UPDATE [Registration] 
    SET [ApprenticeshipType] = 0
    WHERE [ApprenticeshipType] IS NOT NULL  -- Skip NULL values if any

    -- Verify counts
    DECLARE @RevCount INT = @@ROWCOUNT
    DECLARE @RegCount INT = (SELECT COUNT(*) FROM [Registration] WHERE [ApprenticeshipType] = 1)
    
    PRINT 'Updated ' + CAST(@RevCount AS VARCHAR) + ' rows in Revision table'
    PRINT 'Updated ' + CAST(@RegCount AS VARCHAR) + ' rows in Registration table'

    -- Uncomment to automatically rollback for testing:
    -- RAISERROR('Testing rollback', 16, 1)

    COMMIT TRANSACTION
    PRINT 'Update successful! Transaction committed.'
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION
    
    DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE()
    DECLARE @ErrorSeverity INT = ERROR_SEVERITY()
    DECLARE @ErrorState INT = ERROR_STATE()
    
    RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState)
    
    -- Check if backups exist
    IF OBJECT_ID('tempdb..#Revision_Backup') IS NOT NULL
    BEGIN
        PRINT 'Original Revision data preserved in #Revision_Backup'
    END
    IF OBJECT_ID('tempdb..#Registration_Backup') IS NOT NULL
    BEGIN
        PRINT 'Original Registration data preserved in #Registration_Backup'
    END
END CATCH