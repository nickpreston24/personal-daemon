#!/bin/bash
sudo dotnet publish "personal-daemon.csproj" -c Release -o ~/personal-daemon/srv/personal-daemon # release to your user directory:

# sample:
# dotnet publish -c Release -o /srv/HelloWorld
