using System.Data;
using CodeMechanic.FileSystem;
using CodeMechanic.Scriptures;
using CodeMechanic.Types;
using CodeMechanic.Youtube;
using Coravel;
using Dapper;
using Insight.Database.Providers.MySql;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;

namespace personal_daemon;

public class Program
{
    public static async Task Main(string[] args)
    {
        DotEnv.Load(debug: true);

        MySqlInsightDbProvider
            .RegisterProvider(); // not working, see: https://github.com/jonwagner/Insight.Database/issues/441

        await RunAsCoravelScheduler(args);
    }

    private static async Task RunAsCoravelScheduler(string[] args)
    {
        // FS_2.SaveAs(Directory.GetCurrentDirectory(), "logs", "scheduler.log", "test message".AsArray());
        // Changed to return the IHost
        // builder before running it.
        IHost host = CreateHostBuilder(args).Build();
        host.Services.UseScheduler(scheduler =>
            {
                scheduler.Schedule<MyFirstInvocable>()
                    .EveryTenSeconds()
                    // .Weekday()
                    // .Once()
                    ;

                bool rescheduler_enabled =
                    Environment.GetEnvironmentVariable("TODOIST_RESCHEDULER_ENABLED").ToBoolean();
                int seconds =
                    Environment.GetEnvironmentVariable("TODOIST_RESCHEDULER_MINUTES")
                        .ToInt(10);

                if (rescheduler_enabled)
                    scheduler.Schedule<TodoistInvocable>()
                        .EverySeconds(seconds) // todo: update to minutes when in production.
                        .Weekday()
                        // .When(async (_) => rescheduler_enabled)
                        ;

                bool scriptures_enabled = Environment
                        .GetEnvironmentVariable("SCRIPTURES_ENABLED")
                        .ToBoolean()
                    // .Dump("scriptures enabled?")
                    ;

                Console.WriteLine("Scriptures enabled? " + scriptures_enabled);

                if (scriptures_enabled)
                {
                    int search_scriptures_minutes =
                        Environment.GetEnvironmentVariable("SCRIPTURES_SEARCH_INTERVAL_MINUTES").ToInt(10);
                    Console.WriteLine("setting up scripture invokable...");
                    scheduler.Schedule<ScriptureInvocable>()
                        .EverySeconds(search_scriptures_minutes) // todo: update to minutes when in production.
                        .Weekday()
                        .Once()
                        ;
                }
            })
            .OnError(async (exception) =>
                await TemporaryExceptionLogger.LogException(exception)
            );
        host.Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .UseSystemd() // src:  https://devblogs.microsoft.com/dotnet/net-core-and-systemd/?WT.mc_id=ondotnet-c9-cephilli
            .ConfigureServices((hostContext, services) =>
            {
                // Insight.Database.Providers.MySql.MySqlInsightDbProvider.RegisterProvider();

                services.AddScheduler();

                services.AddSingleton<ICachedArgsService>(new CachedArgsService(args));

                services.AddSingleton<IYoutubeService, YoutubeService>();
                services.AddSingleton<IRaindropService, RaindropService>();
                services.AddSingleton<ITodoistSchedulerService, TodoistSchedulerService>();
                services.AddSingleton<IScriptureService, ScriptureService>();

                // Add this 👇
                services.AddTransient<MyFirstInvocable>();
                services.AddTransient<YoutubeInvocable>();
                services.AddTransient<TodoistInvocable>();
                services.AddTransient<ScriptureInvocable>();
            })
            .ConfigureLogging((hostingContext, logging) =>
            {
                logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                logging.AddConsole();
            });
}