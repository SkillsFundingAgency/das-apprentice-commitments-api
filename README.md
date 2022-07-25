# das-github-template

This repo should be used as a template when creating new repos for the Apprenticeship Service

## Contents

* .github/CODEOWNERS - Defines required approvals for changes to files specified in the CODEOWNERS file
* azure/template.json - Azure ARM template should be used to provision resources on the Azure platform
* .gitignore - Intialised for Visual Studio
* azure-pipelines.yml - Azure Pipelines definition file
* GitVersion.yml - GitVersion configuration file
* LICENSE - License information file
* README.md - Populate with useful information about the repo, the projects it contains and how to get started.
# ProjectName

## Introduction

This repo has the majority of the api endpoints to perform crud operaions on cmad entities.  

## Developer Setup

Requires that the standard das-employer-config configuration is setup and running locally in Azure Storage Emiulator

The repo has a database project which should be deployed locally to a database named ‘SFA.DAS.ApprenticeCommitments.Database’

runs on: https://localhost:5501/index.html

The SFA.DAS.ApprenticeCommitments.Api project should be run in kestrel to ensure that the 5501 port is used.

also see onboarding guide [here](https://skillsfundingagency.atlassian.net/wiki/spaces/NDL/pages/3518529551/Apprentice+Portal+-+on+boarding+guide)

### Requirements

### Setup

### Config

local appsettings.development.json file:

```json
{
  "ApplicationSettings": {
    "DbConnectionString": "Data Source=.;Initial Catalog=SFA.DAS.ApprenticeCommitments.Database;Integrated Security=True;Pooling=False;Connect Timeout=30",
    "NServiceBusConnectionString": "UseLearningEndpoint=true",
    "NServiceBusLicense": "",
    "TimeToWaitBeforeChangeOfApprenticeshipEmail": "24:00:00",
    "FuzzyMatchingSimilarityThreshold": 49
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "NServiceBusConnectionString": "UseLearningEndpoint=true",
  "NServiceBusLicense": "",
  "AzureAd": {
    "tenant": "citizenazuresfabisgov.onmicrosoft.com",
    "identifier": "https://citizenazuresfabisgov.onmicrosoft.com/das-at-?????-as-ar"
  },
  "DaysUntilCommitmentStatementOverdue": 10
}
```