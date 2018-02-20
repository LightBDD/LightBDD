dotnet build
..\..\.nuget\nuget.exe install NUnit.ConsoleRunner -Version 3.7.0 -NonInteractive -ExcludeVersion -Output packages

@echo.
@echo Running tests

packages\NUnit.ConsoleRunner\tools\nunit3-console.exe bin\Debug\net46\Example.LightBDD.NUnit3.dll