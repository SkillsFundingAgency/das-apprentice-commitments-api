@database
@api

Feature: GetApprenticeshipRevision
	As a application user 
	I want to be able to get an explicit apprenticeship revision from the database

Scenario: When the apprenticeship for a given apprentice exists
	Given the apprenticeship exists and it's associated with this apprentice
	When we try to retrieve the apprenticeship revision
	Then the result should return ok
	And the response should match the expected apprenticeship values

Scenario: When the apprenticeship doesn't exist
	Given there is no apprenticeship revision
	When we try to retrieve the apprenticeship revision
	Then the result should return NotFound