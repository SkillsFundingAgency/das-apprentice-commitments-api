-- This identifies 100 accounts which are assigned to duplicate signins (should contain 100)
SELECT A.Id, A.CreatedOn, R.CommitmentsApprenticeshipId, R.ConfirmedOn, CASE WHEN RE.ApprenticeId = RE.UserIdentityId THEN 1 ELSE 0 END AS Matches,  RE.*
FROM Apprentice A
LEFT JOIN Registration RE ON RE.UserIdentityId = A.Id
JOIN Apprenticeship AA ON AA.ApprenticeId = A.Id
JOIN Revision R ON R.ApprenticeshipId = AA.ID
WHERE R.CommitmentsApprenticeshipId IN 
(
	SELECT O.CommitmentsApprenticeshipId FROM 
	(
	SELECT
		AA.ApprenticeId,
		R.CommitmentsApprenticeshipId,
		Count(*) AS ACount
	FROM (SELECT IR.ApprenticeshipId, MIN(IR.CommitmentsApprenticeshipId) AS CommitmentsApprenticeshipId FROM Revision IR GROUP BY IR.ApprenticeshipId) R
			JOIN Apprenticeship AA ON R.ApprenticeshipId = AA.Id
			JOIN Apprentice A ON A.Id = AA.ApprenticeId
		GROUP BY 
		R.CommitmentsApprenticeshipId, 
		AA.ApprenticeId
	) O 
	GROUP BY O.CommitmentsApprenticeshipId
	HAVING COUNT(*) > 1
)
AND R.CommitmentsApprenticeshipId NOT IN (1386238, 1278843, 1349637, 1349637)
ORDER BY R.CommitmentsApprenticeshipId, A.CreatedOn 



-- This assigns the latest apprenticeId to registration table (inner query as above)
SELECT 
'UPDATE Regristation SET ApprenticeID = ''' + CONCAT(CurrentApprenticeId, '''') + ' WHERE RegistrationId = ''' + CONCAT(RegistrationId, '''') + CHAR(13)+CHAR(10) 
FROM 
(
	SELECT A1.Id AS OldApprenticeId, RE1.RegistrationId, A2.Id AS CurrentApprenticeId, RE2.RegistrationId AS MissingRegistrationId
	FROM 
	(
		SELECT R.CommitmentsApprenticeshipId, MIN(A.CreatedOn) AS FirstCreate, MAX(A.CreatedOn) AS LastCreate
		FROM Apprentice A
		LEFT JOIN Registration RE ON RE.UserIdentityId = A.Id
		JOIN Apprenticeship AA ON AA.ApprenticeId = A.Id
		JOIN Revision R ON R.ApprenticeshipId = AA.ID
		WHERE R.CommitmentsApprenticeshipId IN 
		(
			SELECT O.CommitmentsApprenticeshipId FROM 
			(
			SELECT
				AA.ApprenticeId,
				R.CommitmentsApprenticeshipId,
				Count(*) AS ACount
			FROM (SELECT IR.ApprenticeshipId, MIN(IR.CommitmentsApprenticeshipId) AS CommitmentsApprenticeshipId FROM Revision IR GROUP BY IR.ApprenticeshipId) R
					JOIN Apprenticeship AA ON R.ApprenticeshipId = AA.Id
					JOIN Apprentice A ON A.Id = AA.ApprenticeId
				GROUP BY 
				R.CommitmentsApprenticeshipId, 
				AA.ApprenticeId
			) O 
			GROUP BY O.CommitmentsApprenticeshipId
			HAVING COUNT(*) > 1
		) AND R.CommitmentsApprenticeshipId NOT IN (1386238, 1278843, 1349637, 1349637)
		GROUP BY R.CommitmentsApprenticeshipId
	) MM
	LEFT JOIN Apprentice A1 ON A1.CreatedOn = MM.FirstCreate
	LEFT JOIN Registration RE1 ON RE1.UserIdentityId = A1.Id
	LEFT JOIN Apprentice A2 ON A2.CreatedOn = MM.LastCreate
	LEFT JOIN Registration RE2 ON RE2.UserIdentityId = A2.Id
) RESULT


-- This deletes the old apprenticeship details (inner query as above)
SELECT 
'DELETE Revision WHERE Id IN (SELECT R.Id FROM Apprenticeship AA JOIN Revision R ON R.ApprenticeshipID = AA.Id WHERE AA.ApprenticeId = ''' + CONCAT(OldApprenticeId, '''') + ')' + CHAR(13)+CHAR(10) +
'DELETE Apprenticeship WHERE ApprenticeId = ''' + CONCAT(OldApprenticeId, '''') + CHAR(13)+CHAR(10)
FROM 
(
	SELECT A1.Id AS OldApprenticeId, RE1.RegistrationId, A2.Id AS CurrentApprenticeId, RE2.RegistrationId AS MissingRegistrationId
	FROM 
	(
		SELECT R.CommitmentsApprenticeshipId, MIN(A.CreatedOn) AS FirstCreate, MAX(A.CreatedOn) AS LastCreate
		FROM Apprentice A
		LEFT JOIN Registration RE ON RE.UserIdentityId = A.Id
		JOIN Apprenticeship AA ON AA.ApprenticeId = A.Id
		JOIN Revision R ON R.ApprenticeshipId = AA.ID
		WHERE R.CommitmentsApprenticeshipId IN 
		(
			SELECT O.CommitmentsApprenticeshipId FROM 
			(
			SELECT
				AA.ApprenticeId,
				R.CommitmentsApprenticeshipId,
				Count(*) AS ACount
			FROM (SELECT IR.ApprenticeshipId, MIN(IR.CommitmentsApprenticeshipId) AS CommitmentsApprenticeshipId FROM Revision IR GROUP BY IR.ApprenticeshipId) R
					JOIN Apprenticeship AA ON R.ApprenticeshipId = AA.Id
					JOIN Apprentice A ON A.Id = AA.ApprenticeId
				GROUP BY 
				R.CommitmentsApprenticeshipId, 
				AA.ApprenticeId
			) O 
			GROUP BY O.CommitmentsApprenticeshipId
			HAVING COUNT(*) > 1
		) AND R.CommitmentsApprenticeshipId NOT IN (1386238, 1278843, 1349637, 1349637)
		GROUP BY R.CommitmentsApprenticeshipId
	) MM
	LEFT JOIN Apprentice A1 ON A1.CreatedOn = MM.FirstCreate
	LEFT JOIN Registration RE1 ON RE1.UserIdentityId = A1.Id
	LEFT JOIN Apprentice A2 ON A2.CreatedOn = MM.LastCreate
	LEFT JOIN Registration RE2 ON RE2.UserIdentityId = A2.Id
) RESULT



