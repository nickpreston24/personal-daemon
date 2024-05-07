using CodeMechanic.Youtube;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace personal_daemon;

public class Program
{
    public static async Task Main(string[] args)
    {
        await RunAsDaemon(args);
    }

    private static async Task RunAsDaemon(string[] args)
    {
        var builder =
            Host.CreateDefaultBuilder(args)
                .UseSystemd() // src:  https://devblogs.microsoft.com/dotnet/net-core-and-systemd/?WT.mc_id=ondotnet-c9-cephilli
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddOptions();
                    services.Configure<DaemonConfig>(hostContext.Configuration.GetSection("Daemon"));

                    services.AddSingleton<ICachedArgsService>(new CachedArgsService(args));
                    services.AddSingleton<IYoutubeService, YoutubeService>();
                    services.AddSingleton<IRaindropService, RaindropService>();
                    services.AddSingleton<IHostedService, DaemonService>();
                    services.AddHostedService<DaemonService>();
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    logging.AddConsole();
                });

        await builder.RunConsoleAsync();
    }
}