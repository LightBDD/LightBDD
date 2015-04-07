rmdir -force make\packages -recurse -ErrorAction SilentlyContinue
rmdir -force make\psmake.*.*.*.* -recurse -ErrorAction SilentlyContinue
& $PSScriptRoot\make.ps1 -NuGetSource 'https://www.nuget.org/api/v2/' -t build @args