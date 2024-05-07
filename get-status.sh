echo "fetching current status:"
sudo systemctl status personal-daemon
echo "fetching journal:"
journalctl -xeu personal-daemon.service
