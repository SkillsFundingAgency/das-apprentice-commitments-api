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
	And the Confirmation Commenced event is published

Scenario: A new apprenticeship has been created as a continuation of an apprenticeship
	Given we have an existing apprenticeship
	And we have a update apprenticeship continuation request
	When the update is posted
	Then the result should return accepted
	And the new commitment statement exists in database
	And the new commitment statement has a new commitments apprenticeship Id

Scenario: Update registration details if the apprentice has not confirmed his identity
	Given we do have an unconfirmed registration
	And we have an update apprenticeship request
	When the update is posted
	Then there should be no commitment statements in the database
	And we have updated the apprenticeship details for the unconfirmed registration
	And registration commitments apprenticeship are updated correctly

Scenario: Update registration details with Continuation Id if the apprentice has not confirmed his identity 
	Given we do have an unconfirmed registration
	And we have a update apprenticeship continuation request
	When the update is posted
	Then there should be no commitment statements in the database
	And we have updated the apprenticeship details for the unconfirmed registration
	And registration commitments apprenticeship are updated correctly

Scenario: No apprenticeship or registation details found
	Given we do not have an existing apprenticeship, confirmed or unconfirmed
	And we have an update apprenticeship request
	When the update is posted
	Then the response is bad request
	And a domain exception is thrown

Scenario: No apprenticeship exist, but registation details are marked as completed
	Given we do not have an existing apprenticeship
	And we do have a verified registration 
	And we have an update apprenticeship request
	When the update is posted
	Then the response is bad request
	And a domain exception is thrown
