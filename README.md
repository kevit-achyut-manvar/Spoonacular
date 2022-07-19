# Spoonacular API Databse Task
Spoonacular API Database Task

## Getting Started

In this task, Spoonacular API is meant to be used to get and store various food recipes. This Project has 2 API. 
One API fetches and stores pizza, burger and pasta recipes on daily basis. The other API takes a cuisine and returns a recipe of that cuisine.

## Requirements
You will need install Visual studio with .NetCore and also need Microsoft SQL Server and Microsoft SQL Server Management Studio.

- You can download and install [Visual Studio](https://visualstudio.microsoft.com/downloads/) from here.
- You can download and install [SQL Server](https://www.microsoft.com/en-in/sql-server/sql-server-downloads) from here.
- You can download and install [SSMS](https://docs.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms?view=sql-server-ver16) from here.

## Clone repository from git using this URL

```
$ git clone https://github.com/kevit-achyut-manvar/Spoonacular
```

## Spoonacular Setup

Create an account in Spoonacular API [here](https://spoonacular.com/food-api/console#Dashboard).
Then get your API Key from Profile Section.

## Quick Start

After cloning open project in visual studio and rebuild project for installing all the dependency of a project, all dependency will be added as NugetPackages.
Then setup API Key in appsettings.json file as below
```
"ApiKey" = "?apiKey=<Your Key>",
```
