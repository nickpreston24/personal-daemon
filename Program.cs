using CodeMechanic.FileSystem;
using CodeMechanic.Youtube;
using Coravel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace personal_daemon;

public class Program
{
    public static async Task Main(string[] args)
    {
        DotEnv.Load();
        await RunAsCoravelScheduler(args);
        // await RunAsDaemon(args);
    }

    private static async Task RunAsCoravelScheduler(string[] args)
    {
        // Changed to return the IHost
        // builder before running it.
        IHost host = CreateHostBuilder(args).Build();
        host.Services.UseScheduler(scheduler =>
        {
            // Easy peasy 👇
            // scheduler
            //     .Schedule<MyFirstInvocable>()
            //     .EveryFiveSeconds()
            //     .Weekday();

            // scheduler.Schedule<YoutubeInvocable>()
            //     .EveryFiveSeconds()
            //     .Weekday();

            scheduler.Schedule<TodoistInvocable>()
                .EveryTenSeconds()
                .Weekday();
        });
        host.Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .UseSystemd() // src:  https://devblogs.microsoft.com/dotnet/net-core-and-systemd/?WT.mc_id=ondotnet-c9-cephilli
            .ConfigureServices((hostContext, services) =>
            {
                services.AddScheduler();
                services.AddSingleton<ICachedArgsService>(new CachedArgsService(args));
                services.AddSingleton<IYoutubeService, YoutubeService>();
                services.AddSingleton<IRaindropService, RaindropService>();
                services.AddSingleton<ITodoistSchedulerService, TodoistSchedulerService>();

                // Add this 👇
                services.AddTransient<MyFirstInvocable>();
                services.AddTransient<YoutubeInvocable>();
                services.AddTransient<TodoistInvocable>();
            });


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
                    services.AddSingleton<IHostedService, DaemonWorker>();
                    services.AddHostedService<DaemonWorker>();

                    services.AddScheduler();

                    // services.UseScheduler(scheduler => {
                    //     // We'll fill this in later ;)
                    // });
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    logging.AddConsole();
                });

        await builder.RunConsoleAsync();
    }
}