using System.Diagnostics;
using Coravel.Invocable;

namespace personal_daemon;

public class TodoistInvocable : IInvocable
{
    private readonly ITodoistSchedulerService todoist;

    public TodoistInvocable(ITodoistSchedulerService svc)
    {
        todoist = svc;
    }

    // string[] ids = new[] { "7966526318" };
    public async Task Invoke()
    {
        var watch = Stopwatch.StartNew();
        Console.WriteLine("Rescheduling todos ...");

        var search = new TodoistTaskSearch()
        {
            label = "bump"
        };

        var todos = (await todoist.SearchTodos(search)).ToArray();
        var rescheduled_todos = await todoist.Reschedule(todos);

        watch.Stop();
        Console.WriteLine($"Rescheduling complete. {rescheduled_todos.Count} todos were updated.");
        Console.WriteLine($"Completed in {watch.ElapsedMilliseconds} milliseconds.");
    }
}