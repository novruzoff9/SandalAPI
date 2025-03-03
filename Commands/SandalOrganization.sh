#!/bin/bash

cd ..
docker build -t sandalorganization -f Services/Organization/Organization.WebAPI/Dockerfile .
docker tag sandalorganization novruzoff999/sandalorganization:latest
docker push novruzoff999/sandalorganization:latest

sleep 3