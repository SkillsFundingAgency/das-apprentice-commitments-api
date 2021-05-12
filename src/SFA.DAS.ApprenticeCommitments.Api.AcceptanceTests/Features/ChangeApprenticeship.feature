@database
@api

Feature: ChangeApprenticeship

Scenario: Update an apprenticeship with a new commitment statement
	Given we have an existing apprenticeship
	And we have an update apprenticeship request
	When the update is posted
	Then the result should return accepted
	And the new commitment statement exists in database
	And the new commitment statement has same commitments apprenticeship Id

Scenario: Ignore updates to apprenticeships we do not know
	Given we do not have an existing apprenticeship
	And we have an update apprenticeship request
	When the update is posted
	Then there should be no commitment statements in the database

Scenario: A new apprenticeship has been created as a continuation of an apprenticeship
	Given we have an existing apprenticeship
	And we have a update apprenticeship continuation request
	When the update is posted
	Then the result should return accepted
	And the new commitment statement exists in database
	And the new commitment statement has a new commitments apprenticeship Id

Scenario: Ignore a new apprenticeship has been created as a continuation of an apprenticeship, when we do not know it
	Given we do not have an existing apprenticeship
	And we have a update apprenticeship continuation request
	When the update is posted
	Then the result should return accepted
	And there should be no commitment statements in the database
