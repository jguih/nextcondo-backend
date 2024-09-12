#!bin/bash

parent_path=$( cd "$(dirname "${BASH_SOURCE[0]}")" ; pwd -P )
root_path="${parent_path}/.."

run_tests() {
  dotnet test
}

build_docker_image() {
  cd "App"
  docker build -t thejguih/nextcondoapi:latest .
}

finish() {
  local result=$?
  exit "${result}"
}

finish_err() {
  local result=$?
  printf "Failed to build dotnetapp docker image \n"
  exit "${result}"
}

trap finish_err ERR
trap finish EXIT

cd "${root_path}"

bash Scripts/migrate.sh
run_tests
build_docker_image