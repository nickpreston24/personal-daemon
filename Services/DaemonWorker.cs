using System.Diagnostics;
using System.Text;
using CodeMechanic.Diagnostics;
using CodeMechanic.FileSystem;
using CodeMechanic.Shargs;
using CodeMechanic.Types;
using CodeMechanic.Youtube;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Systemd;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace personal_daemon;

/// <summary>
/// Adapted from: https://www.atmosera.com/blog/creating-a-daemon-with-net-core-part-1/
/// </summary>
public class DaemonWorker : IHostedLifecycleService, IDisposable
{
    private readonly ILogger logger;
    private readonly IOptions<DaemonConfig> config;
    private readonly IYoutubeService youtube_service;
    private readonly IRaindropService raindrop_service;
    private readonly ICachedArgsService cachedArgsService;
    private readonly bool generate_json;
    private string current_log_file_save_path;
    private int delay_seconds = 5;

    private string[] args { get; set; }

    public DaemonWorker(
        ILogger<DaemonWorker> logger
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

        // WriteLocalLogfile(message.AsArray());

        var sw = Stopwatch.StartNew();
        string cwd = Directory.GetCurrentDirectory();
        Console.WriteLine("Current dir :>> " + cwd);
        // string base_directory = cwd.GoUp(2);

        string base_directory = "/home/nick/Desktop/projects/";
        // string path_root = Path.GetPathRoot(base_directory);
        // string full_path = Path.GetFullPath(base_directory);
        // Console.WriteLine("path_root:>>" + path_root);
        // Console.WriteLine("full_path:>>" + full_path);

        Console.WriteLine("base directory:>>" + base_directory);
        var grep_results_map = await youtube_service.FindAllYoutubeLinks(base_directory,
            debug_mode: false
            // , "Services"
            , "Pages"
            // , "www*"
        );

        sw.Stop();
        Console.WriteLine(sw.Elapsed);

        var links = grep_results_map
                .SelectMany(kvp => kvp.Value
                    .Select(grepResult => grepResult.Line))
                // .Dump("grep results")
                .ToList()
            ;
        Console.WriteLine("total youtube links found :>> " + links.Count);


        var total_ms = sw.ElapsedMilliseconds;
        double ms_per_link = links.Count == 0 ? -1 : total_ms / links.Count;
        Console.WriteLine("ms per link: " + ms_per_link);
        Console.WriteLine("total ms: >> " + total_ms);


        string log_message = new StringBuilder()
            .AppendLine("ms_per_link:" + ms_per_link)
            .ToString();

        // WriteLocalLogfile(log_message);

        var collection = youtube_service.ExtractAllYoutubeLinks(links.ToArray());
        await foreach (var link_set in collection)
        {
            // link_set.Dump();
        }

        // while (true)
        // {
        //     Console.WriteLine("Hello From personal-daemon!");
        //     await Task.Delay(1000 * delay_seconds);
        // }


        // if (generate_json) FasterJsonParsingUsingCodeGenerators();


        // return Task.CompletedTask;
    }

    private void WriteLocalLogfile(params string[] lines)
    {
        if (current_log_file_save_path.IsEmpty())
        {
            var now = DateTime.UtcNow.ToFileTime();
            string cwd = Directory.GetCurrentDirectory();
            string filename = $"personal-daemon_{now}.log";
            string folder_path = Path.Combine(cwd, "logs");
            if (!Directory.Exists(folder_path))
                Directory.CreateDirectory(folder_path);
            string savepath = Path.Combine(cwd, "logs", filename);

            current_log_file_save_path = savepath;
        }
        // todo: use codemechanic's version fo this ..
        // FS.SaveAs(new SaveAs(current_log_file_save_path), lines);
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