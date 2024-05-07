echo "stopping running service ... "
sudo systemctl stop personal-daemon # stop the personal-daemon service to remove any file-locks
echo "service stopped."
echo "Republishing service..."
sudo dotnet publish -c Release -o ~/personal-daemon/srv/personal-daemon # release to your user directory
echo "restarting service..."
sudo systemctl start personal-daemon # start service