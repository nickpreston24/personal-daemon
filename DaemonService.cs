using System.Runtime.Caching;
using CodeMechanic.Diagnostics;
using CodeMechanic.FileSystem;
using CodeMechanic.Shargs;
using CodeMechanic.Types;
using CodeMechanic.Youtube;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Systemd;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace personal_daemon;

/// <summary>
/// Adapted from: https://www.atmosera.com/blog/creating-a-daemon-with-net-core-part-1/
/// </summary>
public class DaemonService : IHostedLifecycleService, IDisposable
{
    private readonly ILogger logger;
    private readonly IOptions<DaemonConfig> config;
    private readonly IYoutubeService youtube_service;
    private readonly IRaindropService raindrop_service;
    private readonly ICachedArgsService cachedArgsService;
    private readonly bool generate_json;

    private string[] args { get; set; }

    public DaemonService(
        ILogger<DaemonService> logger
        , IOptions<DaemonConfig> config
        , IYoutubeService youtubeService
        , IRaindropService raindrop
        , ICachedArgsService cachedArgsService
        , IHostLifetime lifetime
    )
    {
        this.logger = logger;
        logger.LogInformation("IsSystemd: {isSystemd}", lifetime.GetType() == typeof(SystemdLifetime));
        logger.LogInformation("IHostLifetime: {hostLifetime}", lifetime.GetType());


        this.config = config;
        this.youtube_service = youtubeService;
        this.raindrop_service = raindrop;

        this.cachedArgsService = cachedArgsService;

        args = cachedArgsService.GetCachedArgs();
        args.Dump("args from cache");

        var options = new ArgumentsCollection(args);

        options.Dump("options");

        generate_json = options.MatchingFlag("--generate-json");
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        string message = "Starting my super special awesum daemon: " + config.Value.DaemonName;
        logger.LogInformation(message);

        WriteLocalLogfile(message.AsArray());

        // while (true)
        // {
        //     Console.WriteLine("Hello From personal-daemon!");
        //     await Task.Delay(500);
        // }


        // if (generate_json) FasterJsonParsingUsingCodeGenerators();


        // return Task.CompletedTask;
    }

    private static void WriteLocalLogfile(params string[] lines)
    {
        string cwd = Directory.GetCurrentDirectory();
        string filename = "personal-daemon.log";
        string savepath = Path.Combine(cwd, filename);
        File.WriteAllLines(savepath, lines);
        // var fileinfo = FS.SaveAs(new SaveAs("personal-daemon.log")
        // {
        //     create_nonexistent_directory = true, debug = true, save_folder = cwd
        // }, lines.Dump(nameof(lines)));
        // fileinfo.Dump(nameof(fileinfo));
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Stopping daemon.");
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        logger.LogInformation("Disposing....");
    }

    // protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    // {
    //     logger.LogCritical("Critical log on startup.");
    //     while (!stoppingToken.IsCancellationRequested)
    //     {
    //         logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
    //         await Task.Delay(1000, stoppingToken);
    //     }
    // }

    public Task StartingAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting daemon...");
        return Task.CompletedTask;
    }

    public Task StartedAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Started daemon.");
        return Task.CompletedTask;
    }

    public Task StoppingAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Stopping daemon...");
        return Task.CompletedTask;
    }

    public Task StoppedAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Stopped daemon.");
        return Task.CompletedTask;
    }
}