@database
@api

Feature: ChangeApprenticeship

Scenario: Update an apprenticeship with a new commitment statement
	Given we have an existing apprenticeship
	And we have an update apprenticeship request
	When the update is posted
	Then the result should return accepted
	And the registration exists in database