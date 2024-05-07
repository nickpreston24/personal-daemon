cd ~/personal-daemon
sudo cp personal-daemon.service /etc/systemd/system/personal-daemon.service
sudo systemctl daemon-reload
sudo systemctl restart personal-daemon
