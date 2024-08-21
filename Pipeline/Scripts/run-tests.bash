#!bin/bash

VolumeName="nextcondo-test-temp"
DockerComposeFile=${BASH_SOURCE[0]}/../../docker-compose-tests.yaml

docker volume create $VolumeName

docker compose -f $DockerComposeFile up \
  --abort-on-container-exit \
  --exit-code-from testapi

docker compose -f $DockerComposeFile down

docker volume rm $VolumeName