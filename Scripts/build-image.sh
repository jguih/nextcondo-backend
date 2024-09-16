#!bin/bash

parent_path=$( cd "$(dirname "${BASH_SOURCE[0]}")" ; pwd -P )
root_path="${parent_path}/.."

run_tests() {
  dotnet test
}

build_migrations_bundle() {
  dotnet ef migrations bundle \
    -o ./App/release/bundle \
    --force \
    --no-build \
    -r linux-x64 \
    --project App
}

publish_app() {
  dotnet publish App -c Release -o ./App/release /p:UseAppHost=false
}

build_docker_image() {
  docker build -t thejguih/nextcondoapi:latest ./App/.
}

remove_release_folder() {
  if [ -d "App/release" ]
  then
    rm -r App/release
  fi;
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
remove_release_folder
build_migrations_bundle
publish_app
build_docker_image