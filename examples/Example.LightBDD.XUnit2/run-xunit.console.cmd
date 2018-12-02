dotnet build
..\..\.nuget\nuget.exe install xunit.runner.console -Version 2.4.1 -NonInteractive -ExcludeVersion -Output packages

@echo.
@echo Running tests

packages\xunit.runner.console\tools\net452\xunit.console.exe bin\Debug\net46\Example.LightBDD.XUnit2.dll
dotnet packages\xunit.runner.console\tools\netcoreapp1.0\xunit.console.dll bin\Debug\netcoreapp1.1\Example.LightBDD.XUnit2.dll