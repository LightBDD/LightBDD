Write-Output @{
	'NuGetConfig' = '.nuget\NuGet.Config';
	'NuGetSource' = "file://$(pwd)/output;https://api.nuget.org/v3/index.json";
}
