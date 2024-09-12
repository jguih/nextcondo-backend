#!bin/bash

parent_path=$( cd "$(dirname "${BASH_SOURCE[0]}")" ; pwd -P )
root_path="${parent_path}/.."

migrate_testdb() {
  export ASPNETCORE_ENVIRONMENT="Tests"
  dotnet ef database update --project App
}

migrate_devdb() {
  export ASPNETCORE_ENVIRONMENT="Development"
  dotnet ef database update --project App
}

finish() {
  local result=$?
  exit "${result}"
}

finish_err() {
  local result=$?
  printf "Failed to apply migrations \n"
  exit "${result}"
}

trap finish_err ERR
trap finish EXIT

cd "${root_path}"

migrate_testdb
migrate_devdb