-- This statement cannot be run on second deployment the code needs to be removed 
UPDATE Revision SET 
	RolesAndResponsibilitiesCorrect = 4
WHERE RolesAndResponsibilitiesCorrect IS NOT NULL

UPDATE Revision SET 
	CreatedOn = CommitmentsApprovedOn
WHERE CreatedOn IS NULL