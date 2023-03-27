$readme = "readme.md"

$solutionName = "AppFiles.sln"
$appFilesFolder = "AppFiles"
$libraryCsproj = "$appFilesFolder\AppFiles.csproj"
$releaseFolder = "$appFilesFolder\bin\Release"

# checks
Write-Host "Checking that this script is being run from the root of the repository..." -ForegroundColor Green
if (!(Test-Path $solutionName)) {
    Write-Host "`tThis script must be run from the root of the repository." -ForegroundColor Red
    exit 1
}
Write-Host "`tOK" -ForegroundColor Green

# Write-Host "Checking that the user is on the main branch..." -ForegroundColor Green
# $branch = git rev-parse --abbrev-ref HEAD
# if ($branch -ne "main") {
#     Write-Host "`tThe user must be on the main branch to release." -ForegroundColor Red
#     exit 1
# }
# Write-Host "`tOK" -ForegroundColor Green
# 
# Write-Host "Checking that the user has no uncommitted changes..." -ForegroundColor Green
# $uncommittedChanges = git status --porcelain
# if ($uncommittedChanges) {
#     Write-Host "`tThe user must have no uncommitted changes to release." -ForegroundColor Red
#     exit 1
# }
# Write-Host "`tOK" -ForegroundColor Green
# 
# Write-Host "Checking that the user has no unpushed changes..." -ForegroundColor Green
# $unpushedChanges = git cherry -v
# if ($unpushedChanges) {
#     Write-Host "`tThe user must have no unpushed changes to release." -ForegroundColor Red
#     exit 1
# }
# Write-Host "`tOK" -ForegroundColor Green

# run tests
Write-Host "Running unit tests..." -ForegroundColor Green
./tools/RunTests.ps1
if ($LASTEXITCODE -ne 0) {
    Write-Host "`tOne or more unit tests failed." -ForegroundColor Red
    exit 1
}
Write-Host "`tOK" -ForegroundColor Green

# release
# clear out the release folder if it exists
Write-Host "Clearing out the release folder..." -ForegroundColor Green
if (Test-Path $releaseFolder) {
    Get-ChildItem -Path $releaseFolder -Recurse | Remove-Item -Force -Recurse
}
Write-Host "`tOK" -ForegroundColor Green

Write-Host "Building the library..." -ForegroundColor Green
dotnet build $libraryCsproj -c Release
if ($LASTEXITCODE -ne 0) {
    Write-Host "`tFailed to build the library." -ForegroundColor Red
    exit 1
}
Write-Host "`tOK" -ForegroundColor Green

# find the nupkg file if it exists
Write-Host "Finding the nupkg folder..." -ForegroundColor Green
$nupkgFile = Get-ChildItem -Path $releaseFolder -Filter "*.nupkg" -Recurse
if ($nupkgFile) {
    Write-Host "`tOK" -ForegroundColor Green
} else {
    Write-Host "`tFailed to find the nupkg folder." -ForegroundColor Red
    exit 1
}

# parse the version from the nupkg file
Write-Host "Parsing the version from the nupkg file..." -ForegroundColor Green
$version = $nupkgFile.Name -replace "AppFiles.", ""
$version = $version -replace ".nupkg", ""
if ($version) {
    Write-Host "`tOK" -ForegroundColor Green
} else {
    Write-Host "`tFailed to parse the version from the nupkg file." -ForegroundColor Red
    exit 1
}

# Replace Version="<old version>" with Version="<new version>" in the readme
Write-Host "Replacing the version in the readme..." -ForegroundColor Green
$readmeText = Get-Content $readme
$readmeText = $readmeText -replace "Version=""\d+\.\d+\.\d+""", "Version=""$version"""
$readmeText | Set-Content $readme
Write-Host "`tOK" -ForegroundColor Green

# publish the nupkg
Write-Host "TODO:" -ForegroundColor Red
Write-Host "Check that the nupkg is correct." -ForegroundColor Red
Write-Host "Publish the nupkg." -ForegroundColor Red
Write-Host "Tag the version." -ForegroundColor Red
Write-Host "Push the tag." -ForegroundColor Red
Write-Host "Update the version in the readme." -ForegroundColor Red
Write-Host "Push the readme." -ForegroundColor Red

exit 1
