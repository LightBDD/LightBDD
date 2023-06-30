dotnet build
..\..\.nuget\nuget.exe install xunit.runner.console -Version 2.4.2 -NonInteractive -ExcludeVersion -Output packages

@echo.
@echo Running tests

packages\xunit.runner.console\tools\net472\xunit.console.exe bin\Debug\net48\Example.LightBDD.XUnit2.dll