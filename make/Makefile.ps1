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

    gci -Filter 'project.json' -Recurse | %{ Replace-InFile $_.fullname $version '"version": "%", //build_ver','"version": "%-pre", //build_ver' }
    gci -Path 'meta-packages' -Filter '*.nuspec' -Recurse | %{ Replace-InFile $_.fullname $version '<version>%</version>','version="[%, )"' }
    Replace-InFile 'AssemblyVersion.cs' $version 'Version("%")'
    Replace-InFile 'QuickStart.txt' $version 'version %!'
    Replace-InFile 'templates\LightBDD.VSIXTemplates\source.extension.vsixmanifest' $version 'Identity Id="d6382c7a-fe20-47e5-b4e1-4d798cef97f1" Version="%"'
    
}

Define-Step -Name 'Build' -Target 'all,build' -Body {
    call dotnet restore
    call "msbuild.exe" LightBDD.sln /t:"Build" /p:Configuration=Release /m /verbosity:m /nologo /p:TreatWarningsAsErrors=true /nr:false
}

Define-Step -Name 'Tests' -Target 'all,test' -Body {
    . (require 'psmake.mod.testing')

    $tests = Define-DotnetTests -TestProject "*.UnitTests"
    $tests += Define-NUnitTests -GroupName "NUnit 2 tests" -TestAssembly "*\bin\Release\*.NUnitTests.dll"
    $tests += Define-DotnetTests -TestProject "*.AcceptanceTests"

    $tests | Run-Tests -EraseReportDirectory -Cover -CodeFilter '+[LightBDD*]* -[*Tests*]*' -TestFilter '*Tests.dll' `
         | Generate-CoverageSummary | Check-AcceptableCoverage -AcceptableCoverage 90
}


Define-Step -Name 'Packaging' -Target 'all,pack' -Body {
    Remove-Item 'output' -Force -Recurse -ErrorAction SilentlyContinue | Out-Null
    mkdir 'output' | Out-Null

    gci -Path "src" -Filter 'project.json' -Recurse `
        | %{ call dotnet pack $_.fullname --output 'output' --no-build --configuration Release}

    gci -Path "meta-packages" -Filter '*.nuspec' -Recurse `
        | %{ call $Context.NugetExe pack $_.fullname -OutputDirectory 'output' }
}

Define-Step -Name 'Prepare templates' -Target 'all,pack' -Body {

    Get-ChildItem '.\templates' -Recurse  -Filter '*.vstemplate' | %{
    Write-Host $_
        $templateDirectory = $_.Directory.FullName
        Write-ShortStatus "Processing: $templateDirectory"
        Copy-Item 'logo\lightbdd.ico' -Destination "$templateDirectory\lightbdd.ico" | Out-Null
    }

    Get-ChildItem '.\templates' -Recurse  -Filter '*.vsixmanifest' | %{
        $templateDirectory = $_.Directory.FullName
        Write-ShortStatus "Processing: $templateDirectory"
        Copy-Item 'logo\lightbdd_small.ico' -Destination "$templateDirectory\lightbdd_small.ico" | Out-Null
    }
}

Define-Step -Name 'Build VSIX package' -Target 'all,pack' -Body {
    call "msbuild.exe" templates\LightBDD.VSIXTemplates.sln /t:"Clean,Build" /p:Configuration=Release /m /verbosity:m /nologo /p:TreatWarningsAsErrors=true /nr:false
    copy-item 'templates\LightBDD.VSIXTemplates\bin\Release\LightBDD.VSIXTemplates.vsix' -Destination 'output'
}