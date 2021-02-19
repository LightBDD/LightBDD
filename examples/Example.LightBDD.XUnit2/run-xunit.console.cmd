dotnet build
..\..\.nuget\nuget.exe install xunit.runner.console -Version 2.4.1 -NonInteractive -ExcludeVersion -Output packages

@echo.
@echo Running tests

packages\xunit.runner.console\tools\net461\xunit.console.exe bin\Debug\net461\Example.LightBDD.XUnit2.dll