using CodeMechanic.Types;
using Coravel.Invocable;

namespace personal_daemon;

public class MyFirstInvocable : IInvocable
{
    public async Task Invoke()
    {
        string message = $"{DateTime.Now.ToDateTimeOffset()} hello from personal-daemon!";
        Console.WriteLine("This is my first invocable (updated)!");

        await TemporaryExceptionLogger.LogException(new Exception("I take exception to that!"));

        // string cwd = Directory.GetCurrentDirectory();
        // string logsfolder = "logs";
        // string subfolder = Path.Combine(cwd, logsfolder);
        // if (!Directory.Exists(subfolder)) Directory.CreateDirectory(subfolder);
        // string full_path = Path.Combine(cwd, logsfolder, "personal-daemon.log");
        // File.AppendAllLines(full_path, message.AsArray());
        // return Task.CompletedTask;
    }
}