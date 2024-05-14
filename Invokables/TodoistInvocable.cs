using Coravel.Invocable;

namespace personal_daemon;

public class TodoistInvocable : IInvocable
{
    private readonly ITodoistSchedulerService todoist;

    public TodoistInvocable(ITodoistSchedulerService svc)
    {
        todoist = svc;
    }

    public async Task Invoke()
    {
        // var sw = Stopwatch.StartNew();
        Console.WriteLine("Rescheduling ids ...");
        string[] ids = new[] { "7966526318" };
        var success = await todoist.Reschedule(ids);
    }
}