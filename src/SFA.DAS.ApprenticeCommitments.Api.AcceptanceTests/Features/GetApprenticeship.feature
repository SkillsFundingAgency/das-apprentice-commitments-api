@database
@api

Feature: GetApprenticeship
	I want to return an apprenticeship for a given apprentice

Scenario: When the apprenticeship for a given apprentice exists
	Given the apprenticeship exists and it's associated with this apprentice
	When we try to retrieve the apprenticeship
	Then the result should return ok
	And the response should match the expected apprenticeship values

Scenario: When the apprenticeship doesn't exist
	Given there is no apprenticeship
	When we try to retrieve the apprenticeship
	Then the result should return NotFound

Scenario: When the apprenticeship exists, but not for this apprentice
	Given the apprenticeship exists, but it's associated with another apprentice
	When we try to retrieve the apprenticeship
	Then the result should return NotFound

Scenario: When multiple apprenticeships for a given apprentice exists
	Given many apprenticeships exists and are associated with this apprentice
	When we try to retrieve the apprenticeship
	Then the result should return ok
	And the response should match the expected apprenticeship values

Scenario: When an apprenticeship with multiple revisions for a given apprentice exists
	Given the apprenticeships exists, has many revisions, and is associated with this apprentice
	When we try to retrieve the apprenticeship
	Then the result should return ok
	And the response should match the newer apprenticeship values
	And all revisions should have the same apprenticeship ID

Scenario: When an apprenticeship with multiple revisions and has previously been confirmed
	Given the apprenticeships exists, has many revisions, and a previous revision has been confirmed
	When we try to retrieve the apprenticeship status
	Then the result should return ok
	And the response should show apprenticeship has been confirmed at least once

Scenario: When an apprenticeship with multiple revisions and has never been confirmed
	Given the apprenticeships exists, has many unconfirmed revisions
	When we try to retrieve the apprenticeship status
	Then the result should return ok
	And the response should show apprenticeship has never been confirmed
