mkdir NuGet
.nuget\NuGet.exe pack -sym LightBDD\LightBDD.csproj -OutputDirectory NuGet -Prop Configuration=Release
.nuget\NuGet.exe pack -sym LightBDD.NUnit\LightBDD.NUnit.csproj -OutputDirectory NuGet -Prop Configuration=Release
.nuget\NuGet.exe pack -sym LightBDD.MbUnit\LightBDD.MbUnit.csproj -OutputDirectory NuGet -Prop Configuration=Release
.nuget\NuGet.exe pack LightBDD.nuspec -OutputDirectory NuGet