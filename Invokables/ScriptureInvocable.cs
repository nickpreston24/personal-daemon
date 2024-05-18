using System.Diagnostics;
using CodeMechanic.Advanced.Regex;
using CodeMechanic.Diagnostics;
using CodeMechanic.FileSystem;
using CodeMechanic.Scriptures;
using CodeMechanic.Types;
using Coravel.Invocable;

namespace personal_daemon;

public class ScriptureInvocable : IInvocable, Coravel.Invocable.ICancellableInvocable
{
    private readonly IScriptureService scriptures_svc;

    public ScriptureInvocable(IScriptureService svc)
    {
        Console.WriteLine(nameof(ScriptureInvocable));
        scriptures_svc = svc;
    }

    public async Task Invoke()
    {
        // Console.WriteLine("Hello from scriptures invokable!");
        try
        {
            bool debug = true;
            var watch = Stopwatch.StartNew();
            Console.WriteLine("Looking for scripture files ...");

            string my_root = Environment.GetEnvironmentVariable("SCRIPTURES_MAIN_ROOT_DIRECTORY");
            string current_root = my_root.NotEmpty() ? my_root : Directory.GetCurrentDirectory();
            if (debug) Console.WriteLine($"Current root set to : '{current_root}'");
            string folder_pattern = "tpot";

            var discovered_directories = await scriptures_svc.SniffDirectories(current_root, folder_pattern);
            discovered_directories.Dump(nameof(discovered_directories));

            var files = await scriptures_svc.GetAllScriptureFiles(current_root, folder_pattern);

            watch.Stop();
            var ms = watch.ElapsedMilliseconds;
            Console.WriteLine($"Took {ms} milliseconds to find {files.Count} files");

            var postfix_scriptures = files
                .SelectMany(fn => fn.Value
                    .Select(gr => gr.Line.Extract<Scripture>(ScriptureRegexPattern.Postfixed.CompiledPattern)))
                .Flatten()
                .ToList();
            
            
            // if (debug)
            // {
            //     files.Select(f => f.Key).Dump("name of first file");
            //     files.Select(f => f.Value).Dump("first file");
            // }

            bool scripture_uploads_enabled = Environment.GetEnvironmentVariable("SCRIPTURE_UPLOADS_ENABLED")
                .ToBoolean(fallback: false);

            if (scripture_uploads_enabled)
            {
                var uploaded_scriptures = await scriptures_svc
                    .UploadScriptures(postfix_scriptures, debug: true);
                Console.WriteLine($"{uploaded_scriptures.Count} total scriptures uploaded.");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public CancellationToken CancellationToken { get; set; }
}