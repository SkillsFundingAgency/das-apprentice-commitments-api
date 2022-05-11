@database
@api

Feature: ChangeRegistration

Scenario: Update an registration with a new revision
	Given we have an existing registration
	And we have an update registration request
	When the update is posted
	Then the result should return OK
	And the new revision exists in database
	And the new revision has same commitments apprenticeship Id
	And the Confirmation Commenced event is published

Scenario: A new registration has been created as a continuation of an registration
	Given we have an existing registration
	And we have a update registration continuation request
	When the update is posted
	Then the result should return OK
	And the new revision exists in database
	And the new revision has a new commitments apprenticeship Id

Scenario: Update registration details if the apprentice has not confirmed his identity
	Given we do have an unconfirmed registration
	And we have an update registration request
	When the update is posted
	Then there should be no revisions in the database
	And we have updated the registration details for the unconfirmed registration
	And registration commitments are updated are updated correctly

Scenario: Update registration details with Continuation Id if the apprentice has not confirmed his identity 
	Given we do have an unconfirmed registration
	And we have a update registration continuation request
	When the update is posted
	Then there should be no revisions in the database
	And we have updated the registration details for the unconfirmed registration
	And registration commitments are updated are updated correctly

Scenario: No registration or registation details found
	Given we do not have an existing registration, confirmed or unconfirmed
	And we have an update registration request
	When the update is posted
	Then the result should return OK
	And a new registration record should exist with the correct information
	And send a Apprenticeship Registered Event

Scenario: No registration exist, but registation details are marked as completed
	Given we do not have an existing registration
	And we do have a verified registration 
	And we have an update registration request
	When the update is posted
	Then the response is bad request
	And a domain exception is thrown: "Cannot update registration as user has confirmed their identity"

Scenario: Do not update an registration when no consequential change is made
	Given we have an existing registration
	And we have an update registration request with no material change
	When the update is posted
	Then the result should return OK
	Then there should only be the original revision in the database
	And no Confirmation Commenced event is published

Scenario: Notify user of change
	Given we have an existing registration
	And we have an update registration request
	When the update is posted
	Then send a Change of Circumstance email to the user

Scenario: Trying to update registration which doesn't have an email
	Given we do not have an existing registration, confirmed or unconfirmed
	And we have an update registration request without an email
	When the update is posted
	Then the response is bad request
	And a validation exception is thrown for the field: "Email" 

Scenario: Stopped Date should be removed from registration details when Continuation Id is present and the apprentice has not confirmed his identity 
	Given we do have an unconfirmed registration which is stopped
	And we have a update registration continuation request
	When the update is posted
	Then stopped date should be removed from registration

Scenario: Stopped Date should NOT be removed from registration details when Continuation Id is NOT present and the apprentice has not confirmed his identity 
	Given we do have an unconfirmed registration which is stopped
	And we have an update registration request
	When the update is posted
	Then stopped date should NOT be removed from registration
