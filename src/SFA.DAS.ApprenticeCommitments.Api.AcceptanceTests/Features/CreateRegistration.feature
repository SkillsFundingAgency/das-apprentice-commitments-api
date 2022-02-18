@database
@api

Feature: CreateRegistration
	As a application user
	I want to create a valid registration in the database

Scenario: Trying to create a registration with invalid values
	Given we have an invalid registration request
	When the registration is posted
	Then the result should return bad request
	And the content should contain error list

Scenario: Trying to create a registration with valid values
	Given we have a valid registration request
	When the registration is posted
	Then the result should return OK
	And the registration exists in database
	And the Confirmation Commenced event is published
