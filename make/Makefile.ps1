Define-Step -Name 'Update version' -Target 'build' -Body {
    function Replace-InFile([string]$file, [string]$version, [string[]]$matchings)
    {
        Write-ShortStatus "Updating $file with $version..."
        $content = Get-Content $file -Encoding UTF8
        foreach($match in $matchings)
        {
            $from = [Regex]::Escape($match) -replace '%','[0-9]+(\.[0-9]+)*'
            $to = $match -replace '%',$version

            $content = $content -replace $from,$to
        }
        Set-Content $file -Value $content -Encoding UTF8
    }

	$version = (Get-Content 'make\current_version').Trim()
    Write-ShortStatus "Updating version to $version..."

    Replace-InFile 'AssemblyVersion.cs' $version 'Version("%")'
    Replace-InFile 'LightBDD.nuspec' $version '<version>%</version>','<dependency id="LightBDD.NUnit" version="%" />'
    Replace-InFile 'LightBDD.VSPackage\source.extension.vsixmanifest' $version 'Identity Id="d6382c7a-fe20-47e5-b4e1-4d798cef97f1" Version="%"'
    
}

Define-Step -Name 'Build' -Target 'build' -Body {
	call "${env:ProgramFiles(x86)}\MSBuild\12.0\Bin\msbuild.exe" LightBDD.sln /t:"Clean,Build" /p:Configuration=Release /m /verbosity:m /nologo /p:TreatWarningsAsErrors=true
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

Define-Step -Name 'Packaging' -Target 'pack' -Body {
	Remove-Item NuGet -Force -Recurse -ErrorAction SilentlyContinue | Out-Null
	mkdir NuGet | Out-Null
	.nuget\NuGet.exe pack -sym LightBDD\LightBDD.csproj -OutputDirectory NuGet -Prop Configuration=Release
	.nuget\NuGet.exe pack -sym LightBDD.NUnit\LightBDD.NUnit.csproj -OutputDirectory NuGet -Prop Configuration=Release
	.nuget\NuGet.exe pack -sym LightBDD.MbUnit\LightBDD.MbUnit.csproj -OutputDirectory NuGet -Prop Configuration=Release
	.nuget\NuGet.exe pack -sym LightBDD.MsTest\LightBDD.MsTest.csproj -OutputDirectory NuGet -Prop Configuration=Release
	.nuget\NuGet.exe pack LightBDD.nuspec -OutputDirectory NuGet
}

Define-Step -Name 'Prepare templates' -Target 'pack' -Body {
	function ZipDirectory ($zipfilename, $sourcedir)
	{
	   Add-Type -Assembly System.IO.Compression.FileSystem
	   $compressionLevel = [System.IO.Compression.CompressionLevel]::Optimal
	   [System.IO.Compression.ZipFile]::CreateFromDirectory($sourcedir,
			$zipfilename, $compressionLevel, $false)
	}

    Remove-Item -Force -Recurse 'Templates\*\*'
	Copy-Item NuGet\LightBDD.NUnit*.nupkg -Exclude *.symbols.nupkg -Destination 'TemplatesSource\ProjectTemplates\LightBDD for NUnit\'
	Copy-Item NuGet\LightBDD.Core*.nupkg -Exclude *.symbols.nupkg -Destination 'TemplatesSource\ProjectTemplates\LightBDD for NUnit\'
	Copy-Item packages\NUnit.2.6.3\NUnit*.nupkg -Exclude *.symbols.nupkg -Destination 'TemplatesSource\ProjectTemplates\LightBDD for NUnit\'

	Get-ChildItem '.\TemplatesSource' -Recurse  -Filter '*.vstemplate' | %{
        $srcDirectory = $_.Directory.FullName
        $packageName = $_.Directory.Name
        $dstDirectory = $_.Directory.Parent.FullName.Replace('\TemplatesSource\','\Templates\')
        Write-ShortStatus $_.FullName
        Copy-Item 'logo\lightbdd.ico' -Destination "$srcDirectory\logo.ico" | Out-Null
        mkdir $dstDirectory -ErrorAction SilentlyContinue | Out-Null
        ZipDirectory "$dstDirectory\$($packageName).zip" $srcDirectory
        Remove-Item "$srcDirectory\logo.ico" | Out-Null
    }
    Remove-Item 'TemplatesSource\ProjectTemplates\LightBDD for NUnit\*.nupkg'
}

Define-Step -Name 'Build VSIX package' -Target 'vsix' -Body {
	call "${env:ProgramFiles(x86)}\MSBuild\12.0\Bin\msbuild.exe" LightBDD.VSPackage.sln /t:"Clean,Build" /p:Configuration=Release /m /verbosity:m /nologo /p:TreatWarningsAsErrors=true
}