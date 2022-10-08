$configuration = "Release"
$verbosity = "m"
$versionSuffix = "beta"
$runTest = $false
$runPack = $true

$dateFormat = [System.DateTime]::Now.ToString("yyyyMMdd.Hmm")
$versionSuffix = "$($versionSuffix).$($dateFormat)"

dotnet restore "$PSScriptRoot/../src/All.sln" --verbosity $verbosity
Write-Host "Restore complete" -ForegroundColor Green

dotnet build "$PSScriptRoot/../src/All.sln" --verbosity $verbosity -c $configuration --no-restore
Write-Host "Build complete" -ForegroundColor Green

if($runTest){
  dotnet test "$PSScriptRoot/../src/All.sln" --verbosity $verbosity -c $configuration --no-build 
  Write-Host "Test complete" -ForegroundColor Green
}

if($runPack){
  if((Test-Path "$PSScriptRoot/../artifacts/nupkg") -eq $false) { New-Item -ItemType Directory -Path "$PSScriptRoot/../artifacts/nupkg" }

  dotnet pack "$PSScriptRoot/../src/Amusoft.Toolkit.System.CommandLine/Amusoft.Toolkit.System.CommandLine.csproj" --verbosity $verbosity -c $configuration -o ../artifacts/nupkg --no-build /p:VersionSuffix=$versionSuffix
      
  $nupkgFiles = Get-ChildItem -Filter "*.nupkg" -Path "$PSScriptRoot/../artifacts/nupkg" | Sort-Object -Descending CreationTime | % {$_.FullName}
  $latest = $nupkgFiles | Select-Object -First 1

  if($latest){
    if((Test-Path "$PSScriptRoot/../artifacts/feed") -eq $false){
      Write-Host "Initializing local feed" -ForegroundColor Green
      nuget init "$PSScriptRoot/../artifacts/nupkg" "$PSScriptRoot/../artifacts/feed"
    } else {
      nuget add $latest -source "$PSScriptRoot/../artifacts/feed"
    }
    Remove-Item $latest
  }
}
