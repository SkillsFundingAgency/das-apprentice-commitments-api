@database
@api

Feature: ChangeApprenticeship

Scenario: Update an apprenticeship with a new commitment statement
	Given we have an existing apprenticeship
	And we have an update apprenticeship request
	When the update is posted
	Then the result should return accepted
	And the new commitment statement exists in database

Scenario: Ignore updates to apprenticeships we do not know
	Given we do not have an existing apprenticeship
	And we have an update apprenticeship request
	When the update is posted
	Then there should be no commitment statements in the database
