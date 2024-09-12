$ScriptDir = Split-Path ${MyInvocation}.MyCommand.Path -Parent

function Start-MigrationTests {
  $env:ASPNETCORE_ENVIRONMENT = "Tests"
  Set-Location "${ScriptDir}\.."
  Invoke-Command -ScriptBlock { dotnet ef database update --project App }
}

function Start-MigrationDev {
  $env:ASPNETCORE_ENVIRONMENT = "Development"
  Set-Location "${ScriptDir}\.."
  Invoke-Command -ScriptBlock { dotnet ef database update --project App }
}


Start-MigrationTests
Start-MigrationDev