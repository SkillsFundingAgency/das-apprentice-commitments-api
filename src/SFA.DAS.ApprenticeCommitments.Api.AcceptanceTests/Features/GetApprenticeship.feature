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

Scenario: When an apprenticeship has a change of employer
	Given the apprenticeships exists, has a change of employer, and is associated with this apprentice
	When we try to retrieve the apprenticeship
	Then the result should return ok
	And the apprenticeship revisions collection should indicate a change of employer

Scenario: When an apprenticeship has a change of provider
	Given the apprenticeships exists, has a change of provider, and is associated with this apprentice
	When we try to retrieve the apprenticeship
	Then the result should return ok
	And the apprenticeship revisions collection should indicate a change of provider

Scenario: When an apprenticeship has a change of delivery model
	Given the apprenticeships exists, has a change of delivery model, and is associated with this apprentice
	When we try to retrieve the apprenticeship
	Then the result should return ok
	And the apprenticeship revisions collection should indicate a change of delivery model
