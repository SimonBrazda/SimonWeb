#!/bin/bash

# Author: Simon Brazda
# Date: 8/12/2020
# Description: Builds docker image of SimonWeb app and sends it to Docker Hub and runs script "run_ipdated_simonweb.sh" which runs the image on a server

cd /mnt/c/Program\ Files/Docker/Docker
./Docker\ Desktop.exe > /dev/null 2>&1 &
sleep 20s
cd /mnt/d/Dev/c#_projects/Web
if docker build -f ./SimonWebMVC/Dockerfile -t rbrazda/simonweb:latest .
then
	echo "Successfuly built the image: rbrazda/simonweb:latest"
	docker login docker.io
	if docker push rbrazda/simonweb:latest
	then
		echo "Image was successfuly send to your Docker Hub"
	else
	        echo "Failed to send the image to your Docker Hub"
		exit 1
	fi
else
	echo "Failed to build the image"
	exit 1
fi
ssh -t admin@192.168.0.5 -p 24 sudo -S -i /usr/bin/run_updated_simonweb.sh
