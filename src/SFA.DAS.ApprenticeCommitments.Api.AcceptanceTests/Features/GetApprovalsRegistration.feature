@database
@api

Feature: GetApprovalsRegistration
	As a application user
	I want to retrieve the registration details for a approvals apprenticeship

Scenario: Getting a registration which does exist
	Given there is a registration with an <ApprenticeId> assigned to it
	When we try to retrieve the registration using approvals identity
	Then the result should return ok
	And the response should match the registration in the database with <ExpectedHasApprenticeAssigned>

Examples:
	| ApprenticeId                         | ExpectedHasApprenticeAssigned |
	| 15AC916D-C20A-4521-B7D5-07DB472C26BA | true                          |
	|                                      | false                         |

Scenario: Trying to get a registration for an approvals identity which does NOT exist
	Given there is no registration
	When we try to retrieve the registration using approvals identity
	Then the result should return not found

Scenario: Trying to get an approvals registration with an invalid Id
	When we try to retrieve the registration using a bad request format
	Then the result should return bad request

Scenario: Trying to get an approvals registration with an approvals identity of zero
	Given there is an empty commitments apprenticeship id
	When we try to retrieve the registration using approvals identity
	Then the result should return bad request
	And the error must be say approvals apprenticeship identity must be valid
