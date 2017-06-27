Define-Step -Name 'Update version' -Target 'all,build' -Body {
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
    Replace-InFile 'Common.props' $version '<VersionPrefix>%</VersionPrefix>'
    Replace-InFile 'QuickStart.txt' $version 'version %!'
    Replace-InFile 'templates\LightBDD.VSIXTemplates\source.extension.vsixmanifest' $version 'Identity Id="d6382c7a-fe20-47e5-b4e1-4d798cef97f1" Version="%"'
    
}

Define-Step -Name 'Build' -Target 'all,build' -Body {
    Remove-Item 'output' -Force -Recurse -ErrorAction SilentlyContinue | Out-Null
    mkdir 'output' | Out-Null

    call dotnet build /t:"restore,build" '-c' Release /nologo /p:TreatWarningsAsErrors=true
}

Define-Step -Name 'Tests' -Target 'all,test' -Body {
    . (require 'psmake.mod.testing')

    $tests = Define-DotnetTests -TestProject "*.UnitTests.csproj"
    $tests += Define-NUnitTests -GroupName "NUnit 2 tests" -TestAssembly "*\bin\Release\*.NUnitTests.dll"
    $tests += Define-DotnetTests -TestProject "*.AcceptanceTests.csproj"

    $tests | Run-Tests
    <# Code coverage does not work with current opencover (it does not support portable pdb (it will have to be full) and would require -old-style flag but then dotnet test won't detect netcore tests)
    $tests | Run-Tests -EraseReportDirectory -Cover -CodeFilter '+[LightBDD*]* -[*Tests*]*' -TestFilter '*Tests.dll' `
         | Generate-CoverageSummary | Check-AcceptableCoverage -AcceptableCoverage 90
    #>
}

Define-Step -Name 'Prepare templates' -Target 'all,pack' -Body {

    Get-ChildItem '.\templates' -Recurse -Filter '*.vstemplate' | %{
    Write-Host $_
        $templateDirectory = $_.Directory.FullName
        Write-ShortStatus "Processing: $templateDirectory"
        Copy-Item 'logo\lightbdd.ico' -Destination "$templateDirectory\lightbdd.ico" | Out-Null
    }

    Get-ChildItem '.\templates' -Recurse -Filter '*.vsixmanifest' | %{
        $templateDirectory = $_.Directory.FullName
        Write-ShortStatus "Processing: $templateDirectory"
        Copy-Item 'logo\lightbdd_small.ico' -Destination "$templateDirectory\lightbdd_small.ico" | Out-Null
    }
}

Define-Step -Name 'Build VSIX package' -Target 'all,pack' -Body {
    call "msbuild.exe" templates\LightBDD.VSIXTemplates.sln /tv:15.0 /t:"Clean,Build" /p:Configuration=Release /m /verbosity:m /nologo /p:TreatWarningsAsErrors=true /nr:false
    copy-item 'templates\LightBDD.VSIXTemplates\bin\Release\LightBDD.VSIXTemplates.vsix' -Destination 'output'
}