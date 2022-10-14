$configuration = "Release"
$verbosity = "m"
$versionSuffix = "beta"
$runTest = $false
$runPack = $true

$dateFormat = [System.DateTime]::Now.ToString("yyyyMMdd.1HHmm")
$versionSuffix = "$($versionSuffix).$($dateFormat)"

$projects = @(
    "$PSScriptRoot/../src/Amusoft.Toolkit.System.CommandLine/Amusoft.Toolkit.System.CommandLine.csproj",
    "$PSScriptRoot/../src/Amusoft.Toolkit.System.CommandLine.Generator/Amusoft.Toolkit.System.CommandLine.Generator.csproj",
    "$PSScriptRoot/../src/Amusoft.Toolkit.System.CommandLine.Logging/Amusoft.Toolkit.System.CommandLine.Logging.csproj"
)

$sln = "$PSScriptRoot/../src/All.sln"

dotnet restore "$sln" --verbosity $verbosity
Write-Host "Restore complete" -ForegroundColor Green

dotnet build "$sln" --verbosity $verbosity -c $configuration --no-restore
Write-Host "Build complete" -ForegroundColor Green

if($runTest){
  dotnet test "$sln" --verbosity $verbosity -c $configuration --no-build 
  Write-Host "Test complete" -ForegroundColor Green
}

if($runPack){
  if((Test-Path "$PSScriptRoot/../artifacts/nupkg") -eq $false) { New-Item -ItemType Directory -Path "$PSScriptRoot/../artifacts/nupkg" }

  foreach($project in $projects){
    dotnet pack "$project" --verbosity $verbosity -c $configuration -o ../artifacts/nupkg --no-build /p:VersionSuffix=$versionSuffix
    Write-Host "pack $project complete" -ForegroundColor Green
  }  
      
  $nupkgFiles = Get-ChildItem -Filter "*.nupkg" -Path "$PSScriptRoot/../artifacts/nupkg" | Sort-Object -Descending CreationTime | % {$_.FullName}
  $latest = $nupkgFiles | Select-Object -First 1

  if($latest){
    if((Test-Path "$PSScriptRoot/../artifacts/feed") -eq $false){
      Write-Host "Initializing local feed" -ForegroundColor Green
      nuget init "$PSScriptRoot/../artifacts/nupkg" "$PSScriptRoot/../artifacts/feed"
    } else {
        Get-ChildItem -Filter "*.nupkg" -Path "$PSScriptRoot/../artifacts/nupkg" | 
        Sort-Object -Descending CreationTime |
        % {nuget add "$($_.FullName)" -source "$PSScriptRoot/../artifacts/feed"}
    }
    
    Write-Host "Removing nupkg files from artifacts/nupkg" -ForegroundColor Green
    Get-ChildItem -Filter "*.nupkg" -Path "$PSScriptRoot/../artifacts/nupkg" | 
    Sort-Object -Descending CreationTime |
    % {Remove-Item "$($_.FullName)"}
  }
}
