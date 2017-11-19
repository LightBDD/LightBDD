dotnet build
..\..\.nuget\nuget.exe install NUnit.Runners -Version 2.6.4 -NonInteractive -ExcludeVersion -Output packages

@echo.
@echo Running tests

packages\NUnit.Runners\tools\nunit-console.exe bin\Debug\net46\Example.LightBDD.NUnit2.dll