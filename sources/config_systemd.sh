# Copy service file to a System location
sudo cp personal-daemon.service /lib/systemd/system

# Reload SystemD and enable the service, so it will restart on reboots
sudo systemctl daemon-reload 
sudo systemctl enable personal-daemon

# Start service
sudo systemctl start personal-daemon 

# View service status
systemctl status personal-daemon
