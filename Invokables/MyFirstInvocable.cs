using CodeMechanic.Types;
using Coravel.Invocable;

namespace personal_daemon;

public class MyFirstInvocable : IInvocable
{
    public Task Invoke()
    {
        string message = $"{DateTime.Now.ToDateTimeOffset()} hello from personal-daemon!";
        Console.WriteLine("This is my first invocable (updated)!");
        // string cwd = Directory.GetCurrentDirectory();
        // string logsfolder = "logs";
        // string subfolder = Path.Combine(cwd, logsfolder);
        // if (!Directory.Exists(subfolder)) Directory.CreateDirectory(subfolder);
        // string full_path = Path.Combine(cwd, logsfolder, "personal-daemon.log");
        // File.AppendAllLines(full_path, message.AsArray());
        return Task.CompletedTask;
    }
}

public static class FS_2
{
    public static FileInfo SaveAs(
        string directory,
        string subfolder,
        string filename = "personal-daemon.log",
        params string[] lines)
    {
        if (!Path.HasExtension(filename))
            throw new ArgumentException(nameof(filename) + " must have an extension!");
        // string message = $"{DateTime.Now.ToDateTimeOffset()} hello from personal-daemon!";
        // Console.WriteLine("This is my first invocable!");
        string cwd = Directory.GetCurrentDirectory();
        string full_folder = Path.Combine(cwd, subfolder);
        if (!Directory.Exists(full_folder)) Directory.CreateDirectory(full_folder);
        string full_path = Path.Combine(cwd, subfolder, filename);
        File.AppendAllLines(full_path, lines);
        return new FileInfo(full_path);
    }
}