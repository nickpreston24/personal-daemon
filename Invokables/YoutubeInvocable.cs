using System.Diagnostics;
using CodeMechanic.Youtube;
using Coravel.Invocable;

namespace personal_daemon;

public class YoutubeInvocable : IInvocable
{
    private readonly IYoutubeService youtube;

    public YoutubeInvocable(IYoutubeService svc)
    {
        youtube = svc;
    }

    public async Task Invoke()
    {
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
        var grep_results_map = await youtube.FindAllYoutubeLinks(base_directory,
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

        // return Task.CompletedTask;
        // return Task.CompletedTask;
    }
}