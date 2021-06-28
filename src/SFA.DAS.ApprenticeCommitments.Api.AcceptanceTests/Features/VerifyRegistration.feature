@database
@api

Feature: VerifyRegistration
	As an application user
	I want to verify and complete an existing registration

Scenario: A registration is successfully completed
	Given we have an existing registration
	And the request matches registration details
	When we verify that registration
	Then the apprentice record is created
	And an apprenticeship record is created
	And the Confirmation Commenced event is published
	And the registration has been marked as completed
	And the registration CreatedOn field is unchanged
	And a record of the apprentice email address is kept
	And do not send a Change of Circumstance email to the user

Scenario: A registration is submitted with a different email
	Given we have an existing registration
	And the request email does not match
	When we verify that registration
	Then a bad request is returned
	And a email domain error is returned

Scenario: A registration is re-submitted
	Given we have an existing already verified registration
	And the request matches registration details
	When we verify that registration
	Then a bad request is returned
	And an 'already verified' domain error is returned

Scenario: A registration is submitted with invalid values
	Given the verify registration request is invalid
	When we verify that registration
	Then a bad request is returned
	And response contains the expected error messages

Scenario: A registration is submitted with invalid email
	Given the verify registration request is valid except email address
	When we verify that registration
	Then a bad request is returned
	And response contains the expected email error message

Scenario: A registration is submitted which does not exist
	Given we do NOT have an existing registration
	And a valid registration request is submitted
	When we verify that registration
	Then a bad request is returned
	And response contains the not found error message
