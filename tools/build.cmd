@echo off
call:build && call:unit_test_core && call:unit_test_nunit && call:unit_test_mbunit && call:unit_test_mstest && call:acceptance_test && call:report_tests && echo Build finished successfully!
goto:eof

:build
call:_print Building solution
%windir%\Microsoft.NET\Framework64\v4.0.30319\msbuild.exe LightBDD.sln /t:Clean,Build /p:Configuration=Release /m /verbosity:q /nologo /p:TreatWarningsAsErrors=true
goto:eof

:unit_test_core
call:_print Unit testing LightBDD
rmdir reports /S /Q 2>nul
mkdir reports
call:_install_pkg NUnit.Runners 2.6.4 && call:_install_pkg Opencover 4.5.3522 && call:_run_opencover_nunit LightBDD.UnitTests
goto:eof

:unit_test_nunit
call:_print Unit testing LightBDD for NUnit
call:_run_opencover_nunit LightBDD.NUnit.UnitTests
goto:eof

:unit_test_mbunit
call:_print Unit testing LightBDD for MbUnit
call:_install_pkg GallioBundle 3.4.14 && call:_run_opencover_mbunit LightBDD.MbUnit.UnitTests
goto:eof

:unit_test_mstest
call:_print Unit testing LightBDD for MsTest
call:_run_opencover_mstest LightBDD.MsTest.UnitTests
goto:eof

:acceptance_test
call:_print Acceptance testing LightBDD
call:_run_opencover_nunit LightBDD.AcceptanceTests
goto:eof

:report_tests
call:_print Generating coverage reports
call:_install_pkg ReportGenerator 1.9.1.0 && call:_generate_coverage_rpt && call:_check_coverage
goto:eof

:_print
echo.
echo ******************************************************
echo * %*
echo ******************************************************
echo.
goto:eof

:_print_s
echo # %*...
goto:eof

:_install_pkg
call:_print_s Installing %~1 ver. %~2
.nuget\nuget.exe install %~1 -Version %~2 -OutputDirectory packages -Verbosity normal -ExcludeVersion
goto:eof

:_run_opencover_nunit
call:_run_opencover "%~1" "packages\NUnit.Runners\tools\nunit-console.exe" "%~1\bin\Release\%~1.dll /nologo /trace=Error /labels /out:reports/%~1.log /noshadow /domain:single /xml:reports/%~1.xml"
goto:eof

:_run_opencover_mbunit
call:_run_opencover "%~1" "packages\GallioBundle\bin\Gallio.Echo.exe" "%~1\bin\Release\%~1.dll /no-logo /no-progress /verbosity:Quiet /rt:Xml /rd:reports /rnf:%~1"
goto:eof

:_run_opencover_mstest
call:_run_opencover "%~1" "%ProgramFiles(x86)%\Microsoft Visual Studio 11.0\Common7\IDE\mstest.exe" "/testcontainer:%~1\bin\Release\%~1.dll /nologo"
goto:eof

:_run_opencover
@set "OpenCoverPath=packages\OpenCover\OpenCover.Console.exe"
@set "target=%~1"
@set "runner=%~2"
@set "runner_params=%~3"
call:_print_s Running Open Cover for %target%
%OpenCoverPath% -showunvisited -log:Error -register:user -target:"%runner%" -targetargs:"%runner_params%" -filter:"+[LightBDD*]* -[*Tests*]*" -output:reports/%target%_coverage.xml -returntargetcode -coverbytest:"*.Tests.dll"
goto:eof

:_generate_coverage_rpt
call:_print_s Generating coverage reports
set "ReportGeneratorPath=packages\ReportGenerator\ReportGenerator.exe"
%ReportGeneratorPath% -verbosity:error -reports:reports/LightBDD.UnitTests_coverage.xml;reports/LightBDD.NUnit.UnitTests_coverage.xml;reports/LightBDD.MbUnit.UnitTests_coverage.xml;reports/LightBDD.MsTest.UnitTests_coverage.xml;reports/LightBDD.AcceptanceTests_coverage.xml -targetdir:reports/coveragereport -reporttypes:html,xmlsummary
goto:eof

:_check_coverage
call:_print_s Checking coverage level
set "ACCEPTABLE_COVERAGE=95"
findstr /r "<Coverage>[0-9.]*%%</Coverage>" reports\coveragereport\Summary.xml > reports\coveragereport\_coverage.log
for /F "tokens=2 delims=>%%" %%i in (reports\coveragereport\_coverage.log) do set "COVERAGE=%%i"
echo Code coverage is %COVERAGE%%%

if %COVERAGE% LSS %ACCEPTABLE_COVERAGE% (
echo ERROR: %ACCEPTABLE_COVERAGE%%% code coverage is not reached!
exit /B 1
)
goto:eof