cat > personal_daemon.service <<EOF
[Unit]
Description=Demo service
After=network.target

[Service]
ExecStart=/usr/bin/dotnet $(pwd)/bin/personal_daemon.dll 5000
Restart=on-failure

[Install]
WantedBy=multi-user.target
EOF
