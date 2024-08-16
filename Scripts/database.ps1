
param(
  [Parameter()]
  [switch]$Up,
  [Parameter()]
  [switch]$Down,
  [Parameter()]
  [switch]$Migrate
)

$scriptPath = $MyInvocation.MyCommand.Path
$scriptDirectory = Split-Path $scriptPath -Parent
$session = New-PSSession -HostName home.arpa -UserName jose-guilherme

function MigrateDevDb {
  Invoke-Command -ScriptBlock { 
    dotnet ef database update --project App
  }
}

function MigrateTestDb {
  $TestsEnvProps = Get-Content -Path "${scriptDirectory}\..\App\appsettings.Tests.json" -Raw | ConvertFrom-Json
  $DbHost = $TestsEnvProps.DB_HOST
  $DbUsername = $TestsEnvProps.DB_USER
  $DbPassword = $TestsEnvProps.DB_PASSWORD
  $DbDatabase = $TestsEnvProps.DB_DATABASE

  Invoke-Command -ScriptBlock { param($dbhost, $user, $pass, $db)
    dotnet ef database update `
      --project App `
      --connection "Host=$dbhost;Username=$user;Password=$pass;Database=$db"
  } `
    -ArgumentList $DbHost, $DbUsername, $DbPassword, $DbDatabase
}

if ($Up) {
  Invoke-Command `
    -Session $session `
    -ScriptBlock {
    Set-Location ~/repo/services/nextcondo; docker compose -f docker-compose-dev.yaml up -d
  }
}
elseif ($Down) {
  Invoke-Command `
    -Session $session `
    -ScriptBlock {
    Set-Location ~/repo/services/nextcondo; docker compose -f docker-compose-dev.yaml down
  }
}

if ($Migrate -and !$Down) {
  Write-Host "Applying migrations to develoment database..."
  MigrateDevDb
  Write-Host "Applying migrations to test database..."
  MigrateTestDb
}

