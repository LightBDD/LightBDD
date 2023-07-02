dotnet build
..\..\.nuget\nuget.exe install NUnit.ConsoleRunner -Version 3.16.3 -NonInteractive -ExcludeVersion -Output packages

@echo.
@echo Running tests

packages\NUnit.ConsoleRunner\tools\nunit3-console.exe bin\Debug\net48\Example.LightBDD.NUnit3.dll