#!bin/bash

VolumeName="nextcondo-test-temp"

docker volume create $VolumeName

docker compose -f docker-compose-tests.yaml up \
  --abort-on-container-exit \
  --exit-code-from testapi

docker compose -f docker-compose-tests.yaml down

docker volume rm $VolumeName