@api

Feature: HealthCheck
	In order to check if the api is working
	As a application monitor
	I want to be told the status of the api

Scenario: Checking Api is pingable
	Given the api has started
	When the ping endpoint is called
	Then the result should be return okay


Scenario: Checking Api is not healthy
	Given the database is offline
	When the health endpoint is called
	Then the result should not be healthy