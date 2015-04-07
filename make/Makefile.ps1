Define-Step -Name 'Build' -Target 'build' -Body {
	call "$($env:windir)\Microsoft.NET\Framework64\v4.0.30319\msbuild.exe" LightBDD.sln /t:"Clean,Build" /p:Configuration=Release /m /verbosity:m /nologo /p:TreatWarningsAsErrors=true
}

Define-Step -Name 'Quick Tests' -Target 'test' -Body {
	. (require 'psmake.mod.testing')
	Define-NUnitTests 'LightBDD' "*LightBDD.NUnit*\bin\Release\*Tests.dll" | Run-Tests 
}

Define-Step -Name 'Tests' -Target 'build' -Body {
	. (require 'psmake.mod.testing')

	$tests = @()
	$tests += Define-NUnitTests -GroupName 'LightBDD core' -TestAssembly "LightBDD.UnitTests\bin\Release\LightBDD.UnitTests.dll"
	$tests += Define-NUnitTests -GroupName 'LightBDD NUnit' -TestAssembly "LightBDD.NUnit.UnitTests\bin\Release\LightBDD.NUnit.UnitTests.dll"
	$tests += Define-MbUnitTests -GroupName 'LightBDD MbUnit' -TestAssembly "LightBDD.MbUnit.UnitTests\bin\Release\LightBDD.MbUnit.UnitTests.dll"    
	$tests += Define-MsTests -GroupName 'LightBDD MsTest' -TestAssembly "LightBDD.MsTest.UnitTests\bin\Release\LightBDD.MsTest.UnitTests.dll"
	$tests += Define-NUnitTests -GroupName 'LightBDD Acceptance' -TestAssembly "LightBDD.AcceptanceTests\bin\Release\LightBDD.AcceptanceTests.dll"

	$tests | Run-Tests -EraseReportDirectory -Cover -CodeFilter '+[LightBDD*]* -[*Tests*]*' -TestFilter '*Tests.dll' | Generate-CoverageSummary | Check-AcceptableCoverage -AcceptableCoverage 95
}
