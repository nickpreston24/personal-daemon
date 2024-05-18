# Create application
mkdir personal_daemon
cd personal_daemon
dotnet new console

# Change Program.cs
cat > Program.cs <<EOF
using System;
using System.Threading;


namespace personal_daemon
{
  class Program
  {
    static void Main(
      string[] args)
    {
      var sleep = 3000;
      if (args.Length > 0) { int.TryParse(args[0], out sleep); }
      while (true)
      {
        Console.WriteLine($"Working, pausing for {sleep}ms");
        Thread.Sleep(sleep);
      }
    }
  }
}
EOF

# Restore dependencies
dotnet restore

# Publish to a local bin sub directory
dotnet publish --configuration Release --output bin

# Run local to verify all is good
dotnet ./bin/personal_daemon.dll
