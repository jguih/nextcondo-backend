#!bin/bash

docker compose -f docker-compose-tests.yaml up \
  --abort-on-container-exit \
  --exit-code-from testapi

docker compose -f docker-compose-tests.yaml down