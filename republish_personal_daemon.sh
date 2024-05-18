echo "stopping running service ... "
sudo systemctl stop personal_daemon # stop the personal_daemon service to remove any file-locks
echo "service stopped."
echo "Republishing service..."
sudo dotnet publish -c Release -o /srv/personal_daemon # release to your user directory
sudo cp .env /srv/personal_daemon/.env

echo "Updating systemctl ..."
sudo cp personal_daemon.service /etc/systemd/system/personal_daemon.service
sudo systemctl daemon-reload
sudo systemctl start personal_daemon  

echo "restarting service..."
sudo systemctl start personal_daemon # start service
