Define-Step -Name 'Build' -Target 'build' -Body {
	call "$($env:windir)\Microsoft.NET\Framework64\v4.0.30319\msbuild.exe" LightBDD.sln /t:"Clean,Build" /p:Configuration=Release /m /verbosity:m /nologo /p:TreatWarningsAsErrors=true
}

Define-Step -Name 'Unit test LightBDD core' -Target 'build' -Body {
	. (require 'psmake.mod.testing')
		
	Run-NUnitTests -TestAssemblies "LightBDD.UnitTests\bin\Release\LightBDD.UnitTests.dll" -EraseReportDirectory -Cover -CodeFilter '+[LightBDD*]* -[*Tests*]*' -TestFilter '*Tests.dll' -NoCoverageSummary -ReportName 'LightBDD.UnitTests'
}

Define-Step -Name 'Unit test LightBDD.NUnit' -Target 'build' -Body {
	. (require 'psmake.mod.testing')

	Run-NUnitTests -TestAssemblies "LightBDD.NUnit.UnitTests\bin\Release\LightBDD.NUnit.UnitTests.dll" -Cover -CodeFilter '+[LightBDD*]* -[*Tests*]*' -TestFilter '*Tests.dll' -NoCoverageSummary -ReportName 'LightBDD.NUnit.UnitTests'
}

Define-Step -Name 'Unit test LightBDD.MbUnit' -Target 'build' -Body {
	. (require 'psmake.mod.testing')
		
	Run-MbUnitTests -TestAssemblies "LightBDD.MbUnit.UnitTests\bin\Release\LightBDD.MbUnit.UnitTests.dll" -Cover -CodeFilter '+[LightBDD*]* -[*Tests*]*' -TestFilter '*Tests.dll' -NoCoverageSummary -ReportName 'LightBDD.MbUnit.UnitTests'
}


Define-Step -Name 'Unit test LightBDD.MsTest' -Target 'build' -Body {
	. (require 'psmake.mod.testing')
		
	Run-MsTests -TestAssemblies "LightBDD.MsTest.UnitTests\bin\Release\LightBDD.MsTest.UnitTests.dll" -Cover -CodeFilter '+[LightBDD*]* -[*Tests*]*' -TestFilter '*Tests.dll' -NoCoverageSummary -ReportName 'LightBDD.MsTest.UnitTests'
}

Define-Step -Name 'Acceptance test LightBDD' -Target 'build' -Body {
	. (require 'psmake.mod.testing')
		
	Run-NUnitTests -TestAssemblies "LightBDD.AcceptanceTests\bin\Release\LightBDD.AcceptanceTests.dll" -Cover -CodeFilter '+[LightBDD*]* -[*Tests*]*' -TestFilter '*Tests.dll' -NoCoverageSummary -ReportName 'LightBDD.AcceptanceTests'
}

Define-Step -Name 'Generate coverage report' -Target 'build' -Body {
	. (require 'psmake.mod.testing')
	[string[]]$reports = get-childitem reports -Include '*_coverage.xml' -recurse | %{$_.fullname}
	Generate-CoverageSummary -CoverageReports $reports
	Check-AcceptableCoverage -SummaryReport 'reports\summary\summary.xml'  -AcceptableCoverage 95
}
