-- Number Of UserIdentityIds not migrated
select Count(*) from Registration where ApprenticeId IS NULL AND UserIdentityId IS NOT NULL

-- This just makes sure all IDs match
SELECT Count(*) FROM Apprentice WHERE ID IN (select UserIdentityId from Registration where ApprenticeId IS NULL AND UserIdentityId IS NOT NULL)


BEGIN TRANSACTION

-- Replace ApprenticeId with UserIdentityId
UPDATE Registration SET ApprenticeId = UserIdentityId where ApprenticeId IS NULL AND UserIdentityId IS NOT NULL

-- This should now return 0
select Count(*) from Registration where ApprenticeId IS NULL AND UserIdentityId IS NOT NULL

ROLLBACK
--COMMIT
