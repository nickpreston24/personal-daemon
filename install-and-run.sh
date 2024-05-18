#!/bin/bash
bash create-personal-daemon.sh

#rm Program.cs

#cat > Program.cs << EOL
#
#// Replace with a better solution, like Microsoft's Worker (.net 8)
#while(true){
#	Console.WriteLine("Hello World!");
#	await Task.Delay(500);
#}
#
#EOL

bash status.sh
bash enable.sh
bash start.sh
