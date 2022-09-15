@database
@api

Feature: GetMyApprenticeship
	I want to return the latest confirmed apprenticeship for a given apprentice

Scenario: When a confirmed apprenticeship for a given apprentice exists
	Given the apprenticeship exists and it's associated with this apprentice
	When we try to retrieve the latest confirmed apprenticeship
	Then the result should return ok
	And the response should match the last confirmed apprenticeship values

Scenario: When the apprenticeship doesn't exist
	Given there is no apprenticeship
	When we try to retrieve the latest confirmed apprenticeship
	Then the result should return NotFound

Scenario: When the apprenticeship exist, but no revisions have been confirmed
	Given the apprenticeship exists, but no revision has been confirmed
	When we try to retrieve the latest confirmed apprenticeship
	Then the result should return NotFound

Scenario: When multiple apprenticeships for a given apprentice exists
	Given many apprenticeships exists and are associated with this apprentice
	When we try to retrieve the latest confirmed apprenticeship
	Then the result should return ok
	And the response should match the last confirmed apprenticeship values

Scenario: When multiple confirmed revisions for a given apprentice exists
	Given the apprenticeship exists and it's associated with this apprentice
	And many confirmed revisions exist
	When we try to retrieve the latest confirmed apprenticeship
	Then the result should return ok
	And the response should match the last confirmed apprenticeship values