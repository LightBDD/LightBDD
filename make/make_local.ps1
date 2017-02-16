rmdir -force make\psmake.* -recurse -ErrorAction SilentlyContinue
& $PSScriptRoot\make.ps1 -t all @args