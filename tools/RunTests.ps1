$solutionName = "AppFiles.sln"
$unitTestsCsproj = "AppFiles.Tests\AppFiles.Tests.csproj"

# checks
Write-Host "Checking that this script is being run from the root of the repository..." -ForegroundColor Gray
if (!(Test-Path $solutionName)) {
    Write-Host "`tThis script must be run from the root of the repository." -ForegroundColor Red
    exit 1
}
Write-Host "`tOK" -ForegroundColor Green

# run tests
Write-Host "Running unit tests..." -ForegroundColor Gray
dotnet test $unitTestsCsproj
if ($LASTEXITCODE -ne 0) {
    Write-Host "`tOne or more unit tests failed." -ForegroundColor Red
    exit 1
}
Write-Host "`tOK" -ForegroundColor Green
exit 0