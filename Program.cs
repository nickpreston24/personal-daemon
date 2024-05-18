using CodeMechanic.Systemd.Daemons;
using CodeMechanic.Types;
using Coravel;
using Coravel.Invocable;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace personal_daemon;

public class MyFirstInvocable : IInvocable
{
    public async Task Invoke()
    {
        Console.WriteLine("Hello from personal_daemon! (updated at 6am)");
        /// Sample MySQL logging (requires MYSQL_* .env variables to be set in your new .env).
        if (Environment.GetEnvironmentVariable("MYSQLPASSWORD").NotEmpty())
        {
            int rows = await MySQLExceptionLogger.LogInfo($"Invoking from /srv/{nameof(personal_daemon)}!",
                nameof(personal_daemon));
            Console.WriteLine($"{rows} upserted.");
        }
    }
}

public class Program
{
    public static async Task Main(string[] args)
    {
        IHost host = CreateHostBuilder(args)
            .UseSystemd()
            .Build();

        host.Services.UseScheduler(scheduler =>
        {
            // Yes, it's this easy!
            scheduler
                .Schedule<MyFirstInvocable>()
                .EveryFiveSeconds();
            // Console.WriteLine("cool. I loaded the host w/o dying...");
        });

        host.Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices(services =>
            {
                services.AddScheduler();
                services.AddTransient<MyFirstInvocable>();
            });
}