cd ~/personal-daemon
sudo systemctl stop personal-daemon # stop the personal-daemon service to remove any file-locks
dotnet publish -c Release -o /srv/personal-daemon
sudo systemctl start personal-daemon
