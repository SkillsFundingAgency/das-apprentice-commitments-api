-- Gives the total record count of the Revisions we will delete 
SELECT Count(*) FROM Apprentice A 
LEFT JOIN Apprenticeship AA ON AA.ApprenticeId = A.Id
LEFT JOIN Revision R ON R.ApprenticeshipId = AA.Id
WHERE R.LastViewed IS NULL 
AND A.Id in (
	SELECT
		AA.ApprenticeId
	FROM (SELECT IR.ApprenticeshipId, MIN(IR.CommitmentsApprenticeshipId) AS CommitmentsApprenticeshipId FROM Revision IR /* WHERE LastViewed IS NULL */ GROUP BY IR.ApprenticeshipId) R
			JOIN Apprenticeship AA ON R.ApprenticeshipId = AA.Id
			JOIN Apprentice A ON A.Id = AA.ApprenticeId
		GROUP BY 
		R.CommitmentsApprenticeshipId, 
		AA.ApprenticeId
	HAVING Count(*) > 1
) 
-- This excludes CoC which have not been viewed yet, but are stilled linked to an apprenticeship which has been viewed
AND R.ApprenticeshipId NOT IN (SELECT R2.ApprenticeshipID FROM Revision R2 GROUP BY ApprenticeshipId HAVING MAX(R2.LastViewed) IS NOT NULL)



BEGIN TRANSACTION


-- Delete duplicated unseen revisions
DELETE FROM Revision WHERE Id IN 
(
	SELECT R.Id FROM Apprentice A 
	LEFT JOIN Apprenticeship AA ON AA.ApprenticeId = A.Id
	LEFT JOIN Revision R ON R.ApprenticeshipId = AA.Id
	WHERE R.LastViewed IS NULL AND A.Id in (
		SELECT
			AA.ApprenticeId
		FROM (SELECT IR.ApprenticeshipId, MIN(IR.CommitmentsApprenticeshipId) AS CommitmentsApprenticeshipId FROM Revision IR /* WHERE LastViewed IS NULL */ GROUP BY IR.ApprenticeshipId) R
				JOIN Apprenticeship AA ON R.ApprenticeshipId = AA.Id
				JOIN Apprentice A ON A.Id = AA.ApprenticeId
			GROUP BY 
			R.CommitmentsApprenticeshipId, 
			AA.ApprenticeId
		HAVING Count(*) > 1
	)
	-- This excludes CoC which have not been viewed yet, but are stilled linked to an apprenticeship which has been viewed
	AND R.ApprenticeshipId NOT IN (SELECT R2.ApprenticeshipID FROM Revision R2 GROUP BY ApprenticeshipId HAVING MAX(R2.LastViewed) IS NOT NULL)

)

-- delete the apprenticeships which no longer have a revisions
DELETE FROM Apprenticeship  
WHERE Id IN (SELECT AA.Id FROM Apprenticeship AA LEFT JOIN Revision R ON R.ApprenticeshipId = AA.Id WHERE R.ID IS NULL)


-- These 2 delete actions above should match the previous count


-- This should now be 0 (same as first query )
SELECT Count(*) FROM Apprentice A 
LEFT JOIN Apprenticeship AA ON AA.ApprenticeId = A.Id
LEFT JOIN Revision R ON R.ApprenticeshipId = AA.Id
WHERE R.LastViewed IS NULL 
AND A.Id in (
	SELECT
		AA.ApprenticeId
	FROM (SELECT IR.ApprenticeshipId, MIN(IR.CommitmentsApprenticeshipId) AS CommitmentsApprenticeshipId FROM Revision IR /* WHERE LastViewed IS NULL */ GROUP BY IR.ApprenticeshipId) R
			JOIN Apprenticeship AA ON R.ApprenticeshipId = AA.Id
			JOIN Apprentice A ON A.Id = AA.ApprenticeId
		GROUP BY 
		R.CommitmentsApprenticeshipId, 
		AA.ApprenticeId
	HAVING Count(*) > 1
) 
-- This excludes CoC which have not been viewed yet, but are stilled linked to an apprenticeship which has been viewed
AND R.ApprenticeshipId NOT IN (SELECT R2.ApprenticeshipID FROM Revision R2 GROUP BY ApprenticeshipId HAVING MAX(R2.LastViewed) IS NOT NULL)


ROLLBACK
-- COMMIT



