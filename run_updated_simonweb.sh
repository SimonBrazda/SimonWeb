#!/bin/bash

# Author: Simon Brazda
# Date: 8/14/2020
# Description: Pulls new version of simonweb docker image and runs it

sudo -i
docker rm simonweb_old
docker stop simonweb
docker container rename simonweb simonweb_old
if docker pull rbrazda/simonweb:latest
then
        echo "Image has been seccessfuly pulled"
        if docker run -d -p 8080:80 --name simonweb --mount source=simonweb_vol,target=/app rbrazda/simonweb:latest
        then
	        echo "Docker container has been updated successfuly"
                exit 0
        else
                echo "Failed to run the docker image"
                exit 1
	fi
echo "Failed to pull the image";
fi
exit 1
