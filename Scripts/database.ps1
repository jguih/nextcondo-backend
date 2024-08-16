
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
$rootDir = "$scriptDirectory\.."

$session = New-PSSession -HostName home.arpa -UserName jose-guilherme
$testsAppSettingsJsonPath = "${rootDir}\App\appsettings.Tests.json"
$devDockerComposePath = "${rootDir}\docker-compose-dev.yaml"
$devDockerComposeRemotePath = "/tmp/docker-compose-dev.yaml"

function MigrateDevDb {
  Invoke-Command -ScriptBlock { 
    dotnet ef database update --project App
  }
}

function MigrateTestDb {
  $TestsEnvProps = Get-Content -Path $testsAppSettingsJsonPath -Raw | ConvertFrom-Json
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
  Copy-Item -Path $devDockerComposePath -Destination $devDockerComposeRemotePath -ToSession $session
  Invoke-Command `
    -Session $session `
    -ScriptBlock {
    Set-Location /tmp; docker compose -f docker-compose-dev.yaml up -d
  }
  Invoke-Command `
    -Session $session `
    -ScriptBlock { param($path)
    Remove-Item -Path $path
  } `
    -ArgumentList $devDockerComposeRemotePath
}
elseif ($Down) {
  Copy-Item -Path $devDockerComposePath -Destination $devDockerComposeRemotePath -ToSession $session
  Invoke-Command `
    -Session $session `
    -ScriptBlock {
    Set-Location /tmp; docker compose -f docker-compose-dev.yaml down
  }
  Invoke-Command `
    -Session $session `
    -ScriptBlock { param($path)
    Remove-Item -Path $path
  } `
    -ArgumentList $devDockerComposeRemotePath
}

if ($Migrate -and !$Down) {
  Write-Host "Applying migrations to develoment database..."
  MigrateDevDb
  Write-Host "Applying migrations to test database..."
  MigrateTestDb
}

