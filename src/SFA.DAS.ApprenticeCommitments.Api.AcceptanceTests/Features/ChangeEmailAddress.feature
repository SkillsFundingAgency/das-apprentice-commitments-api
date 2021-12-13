@database
@api
Feature: ChangeEmailAddress
	As an apprentice
	I want to be able to change the email address associated with my digital account
	So that I can still access my commitment & receive updates from the service

Scenario: Change to a valid email address and notify linked apprenticeships
	Given we have an existing apprentice with multiple apprenticeships
	And a ChangeEmailCommand with a valid email address
	When we change the apprentice's email address
	Then an ApprenticeEmailAddressedChangedEvent is published for each apprenticeship 
