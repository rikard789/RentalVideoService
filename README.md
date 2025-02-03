# RentalVideoService
Rental Service

Used for project:
 - Visual Studio 2022
 - .NET 8.0
 - Swashbuckle.AspNetCore 7.2.0

Before running project:
1. Create database -> got to folder Database and run:
```
docker-compose up
```
to create mssql database
2. Run migrations to database and add test data 

How to run database migrations(create database and load test data):
1. Open project and go to Backend in powershell terminal
2. Check if after writing 'dotnet ef' and clicking enter you get list of commands with commands like 'migrations' and 'database'
3. If you do not see list then enter commands:
```
dotnet new tool-manifest
dotnet tool install dotnet-ef
```
4. Now you should be able to use commands:
```
dotnet ef migrations add InitialMigration //create Migration
dotnet ef database update                 // update mssql database
```
5. You can check now your database if records were added.


Running unit tests:

```
cd /Backend/test
dotnet test
```

Example output:

![img](/img/unit_test_output.png)