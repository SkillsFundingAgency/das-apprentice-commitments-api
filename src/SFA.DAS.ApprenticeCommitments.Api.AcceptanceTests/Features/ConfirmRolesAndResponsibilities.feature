@database
@api
Feature: ConfirmRolesAndResponsibilities
	As an apprentice I want to be able to confirm my roles and responsibilities

Scenario: Positively confirm roles and responsibilities section - any step
	Given we have an apprenticeship waiting to be confirmed
	And a confirmation stating the roles and responsibilities for have been read for <Confirmation> is set
	When we send the confirmation
	Then the response is OK
	And the apprenticeship record is updated

	Examples: 
	| Confirmation                                |
	| ApprenticeRolesAndResponsibilitiesConfirmed |
	| EmployerRolesAndResponsibilitiesConfirmed   |
	| ProviderRolesAndResponsibilitiesConfirmed   |


Scenario: Positively confirm roles and responsibilities section - step 2
	Given we have an apprenticeship with ApprenticeRolesAndResponsibilitiesConfirmed 
	And a confirmation stating the roles and responsibilities for have been read for <Confirmation> is set
	When we send the confirmation
	Then the response is OK
	And the apprenticeship record now contains ApprenticeRolesAndResponsibilitiesConfirmed and <Confirmation>

	Examples: 
	| Confirmation                              |
	| EmployerRolesAndResponsibilitiesConfirmed |
	| ProviderRolesAndResponsibilitiesConfirmed |

Scenario: Positively confirm roles and responsibilities section - step 3
	Given we have an apprenticeship with ApprenticeRolesAndResponsibilitiesConfirmed and EmployerRolesAndResponsibilitiesConfirmed
	And a confirmation stating the roles and responsibilities for have been read for ProviderRolesAndResponsibilitiesConfirmed is set
	When we send the confirmation
	Then the response is OK
	And the apprenticeship record shows the roles and responsibilities is fully confirmed

Scenario: Attempt to reconfirm a section already confirmed
	Given we have an apprenticeship that has previously had its roles and responsibilities confirmed
	And a confirmation stating the roles and responsibilities for have been read for <Confirmation> is set
	When we send the confirmation
	Then the response is OK
	And the apprenticeship record shows the roles and responsibilities is fully confirmed

	Examples: 
	| Confirmation                                |
	| ApprenticeRolesAndResponsibilitiesConfirmed |
	| EmployerRolesAndResponsibilitiesConfirmed   |
	| ProviderRolesAndResponsibilitiesConfirmed   |