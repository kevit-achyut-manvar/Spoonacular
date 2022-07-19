# Spoonacular API Databse Task
Spoonacular API Database Task

## Getting Started

The goal of this work is to gather and save numerous culinary recipes using the Spoonacular API. This project includes two APIs. Every day, one API retrieves and stores pasta, pizza, and burger recipes. The other API asks for a cuisine and then delivers a recipe for it.

## Requirements
You will need to install Visual studio with .Net Core and also need Microsoft SQL Server and Microsoft SQL Server Management Studio (SSMS).

- You can download offical version of Visual Studio [here](https://visualstudio.microsoft.com/downloads/). Add the ASP.Net and Web Development Package in Visual Studio.
- SQL Server is available [here](https://www.microsoft.com/en-in/sql-server/sql-server-downloads) for download and installation.
- From [this page](https://docs.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms?view=sql-server-ver16), you can download and set up SSMS.

## Clone repository from git using this URL

```
$ git clone https://github.com/kevit-achyut-manvar/Spoonacular.git
```

## Spoonacular Setup

Create an account in Spoonacular API [here](https://spoonacular.com/food-api/console#Dashboard).
Then get your API Key from Profile Section.

## Quick Start

After cloning open project in visual studio and rebuild project for installing all the dependency of a project, all dependency will be added as NugetPackages.
Then complete following steps:
- Edit API Key in appsettings.json file as below
```
"ApiKey" = "?apiKey=<Your Key>",
```
- Edit Connection String in appsettings.json file as below
```
"ConnectionStrings": {
        "DefaultConnection": "Data Source=<Database Server/Host Name>;Initial Catalog=<Database Name>;Integrated Security=True;"
    },
```
- Add the migrations to your database by using following command in Visual Studio Developer Power Shell
```
dotnet ef database update 20220714050631_RecipeSummary
```
```
dotnet ef database update 20220714113915_CuisineRecipeSummary
```

Setup is complete. Now you can run the project (shortcut key F5).
