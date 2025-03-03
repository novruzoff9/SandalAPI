#!/bin/bash

cd ..
docker build -t sandalidentityserver -f IdentityServer/Dockerfile .
docker tag sandalidentityserver novruzoff999/sandalidentityserver:latest
docker push novruzoff999/sandalidentityserver:latest

sleep 3