using System.Runtime.Caching;
using CodeMechanic.Diagnostics;
using CodeMechanic.Shargs;
using CodeMechanic.Youtube;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace personal_daemon;

/// <summary>
/// Adapted from: https://www.atmosera.com/blog/creating-a-daemon-with-net-core-part-1/
/// </summary>
public class DaemonService : IHostedService, IDisposable
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
    )
    {
        this.logger = logger;
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

    public Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting daemon: " + config.Value.DaemonName);
        
        
        
        
        // if (generate_json) FasterJsonParsingUsingCodeGenerators();

        
        return Task.CompletedTask;
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
    
    /// <summary>
    /// src: https://www.youtube.com/watch?v=HhyBaJ7uisU
    /// </summary>
    // private static void FasterJsonParsingUsingCodeGenerators()
    // {
    //     var person = new Person()
    //     {
    //         Name = "Nick Preston",
    //         Age = 34
    //     };
    //
    //     var context = PersonJsonContext.Default.Person;
    //     string text = JsonSerializer.Serialize(person, context);
    //     Console.WriteLine(text);
    //     Person obj = JsonSerializer.Deserialize(text, context);
    //     obj.Dump("person json");
    // }
}