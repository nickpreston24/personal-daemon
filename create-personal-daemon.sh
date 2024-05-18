#!/bin/bash

# src: https://swimburger.net/blog/dotnet/how-to-run-a-dotnet-core-console-app-as-a-service-using-systemd-on-linux

mkdir ~/personal-daemon
cd ~/personal-daemon
dotnet new worker --name personal-daemon

sudo mkdir /srv/
sudo mkdir /srv/personal-daemon               # Create directory /srv/personal-daemon
sudo chown nick /srv/personal-daemon  # Assign ownership to yourself of the directory

cat > personal-daemon.service << EOL
[Unit]
Description=Personal Daemon console application

[Service]
# systemd will run this executable to start the service
# if /usr/bin/dotnet doesn't work, use "which dotnet" to find correct dotnet executable path
ExecStart=/usr/bin/dotnet /srv/personal-daemon/personal-daemon.dll
# to query logs using journalctl, set a logical name here
SyslogIdentifier=personal-daemon

# Use your username to keep things simple.
# If you pick a different user, make sure dotnet and all permissions are set correctly to run the app
# To update permissions, use 'chown nick -R /srv/personal-daemon' to take ownership of the folder and files,
#       Use 'chmod +x /srv/personal-daemon/personal-daemon' to allow execution of the executable file
User=nick

# ensure the service restarts after crashing
Restart=always
# amount of time to wait before restarting the service                        
RestartSec=5   


# This environment variable is necessary when dotnet isn't loaded for the specified user.
# To figure out this value, run 'env | grep DOTNET_ROOT' when dotnet has been loaded into your shell.
Environment=DOTNET_ROOT=/usr/lib64/dotnet

[Install]
WantedBy=multi-user.target
EOL

ls *.service

sudo cp personal-daemon.service /etc/systemd/system/personal-daemon.service
sudo systemctl daemon-reload
sudo systemctl start personal-daemon
sudo journalctl -u personal-daemon


