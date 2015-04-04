$private:PsMakeVer = '3.0.1.0'
$private:PsMakeNugetSource = $null

function private:Get-NuGetArgs ($params, $defaultSource)
{
	function private:Find-Arg($array, $name, $default) { 
		$idx = [array]::IndexOf($array, $name)
		if( ($idx -gt -1) -and ($idx+1 -lt $array.Length)) { return $array[$idx+1] }
		return $default
	}
	$nuGetSource = Find-Arg $params '-NuGetSource' $defaultSource
	if ($nuGetSource) { return '-Source', $nuGetSource}
	return @()
}

$private:srcArgs = Get-NuGetArgs $args $PsMakeNugetSource
.nuget\NuGet.exe install psmake -Version $PsMakeVer -OutputDirectory make -ConfigFile .nuget\NuGet.Config @srcArgs
& "make\psmake.$PsMakeVer\psmake.ps1" -md make @args
